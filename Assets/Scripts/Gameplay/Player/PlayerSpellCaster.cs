using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.UI;+
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Gameplay.Player
{
    [RequireComponent(typeof(PlayerState))]
    public class PlayerSpellCaster : NetworkBehaviour
    {
        class SpellState
        {
            public readonly SpellData Spell;

            private float _timeAvailableToCast;
            private readonly Dictionary<int, SpellHandler> _handlers 
                = new Dictionary<int, SpellHandler>();
            private int _serverCastId = 0;
            private readonly Dictionary<int, int> _queuedEvents = new Dictionary<int, int>(); 

            public bool CanCast
            {
                get { return Time.time > _timeAvailableToCast; }
            }

            public float CurrentCooldown
            {
                get
                {
                    var cd = _timeAvailableToCast - Time.time;
                    return cd > 0 ? cd : 0;
                }
            }

            public SpellState(SpellData spell)
            {
                Spell = spell;
            }

            public int CastServer(PlayerSpellCaster caster, Vector3 position, Vector3 direction)
            {
                _timeAvailableToCast = Time.time + Spell.CoolDown;

                var handler = (SpellHandler)SpellHandler.Instantiate(Spell.Handler);
                handler.Id = _serverCastId++;
                handler.Spell = Spell;
                handler.Caster = caster;

                _handlers.Add(handler.Id, handler);

                Debug.Log(string.Format("Server casting spell:{0} id:{1}", Spell.name, handler.Id));
                handler.Cast(position, direction);

                return handler.Id;
            }

            public void CastClient(int spellHandlerId, PlayerSpellCaster caster, Vector3 position, Vector3 direction)
            {
                //_serverCastId = spellHandlerId;
                _timeAvailableToCast = Time.time + Spell.CoolDown - Network.GetAveragePing(Network.player) / 1000f;

                var handler = (SpellHandler) SpellHandler.Instantiate(Spell.Handler);
                handler.Id = spellHandlerId;
                handler.Spell = Spell;
                handler.Caster = caster;

                _handlers.Add(handler.Id, handler);

                Debug.Log(string.Format("Client casting spell:{0} id:{1}", Spell.name, spellHandlerId));
                handler.Cast(position, direction);

                // process queued events
                if (_queuedEvents.ContainsKey(spellHandlerId))
                {
                    handler.OnSpellEvent(_queuedEvents[spellHandlerId]);
                    _queuedEvents.Remove(spellHandlerId);
                }
            }

            public void SendEvent(int spellHandlerId, int eventId)
            {
                SpellHandler handler;

                if (_handlers.TryGetValue(spellHandlerId, out handler))
                {
                    handler.OnSpellEvent(eventId);
                }
                else
                {
                    Debug.LogFormat("Queued event id:{0} for handler:{1}", eventId, spellHandlerId);
                    _queuedEvents.Add(spellHandlerId, eventId);
                }
            }

            public void DestroyHandler(int spellHandlerId)
            {
                SpellHandler handler;

                if (_handlers.TryGetValue(spellHandlerId, out handler))
                {
                    _handlers.Remove(spellHandlerId);
                    handler.OnDestroy();
                }
                else
                {
                    Debug.LogWarningFormat("Tried to destroy handler but there is no handler with id:{0}", spellHandlerId);
                }
            }
        }

        public List<SpellData> Spells;
    
        public Vector3 Offset = new Vector3();
        public float StartRadius = 0.3f;

        private PlayerState _state;
        private readonly List<SpellState> _spellStates = new List<SpellState>();

        void Start()
        {
            _state = GetComponent<PlayerState>();
            _spellStates.AddRange(Spells.Select(s => new SpellState(s)));

            if (isLocalPlayer && SpellsPanel.Instance != null)
            {
                foreach (var spellData in Spells)
                {
                    SpellsPanel.Instance.RegisterSpell(spellData);
                }
            }
        }

        void Update()
        {
            if (isLocalPlayer && SpellsPanel.Instance != null)
            {
                foreach (var spellState in _spellStates)
                {
                    SpellsPanel.Instance.UpdateCooldown(spellState.Spell, spellState.CurrentCooldown);
                }
            }
        }

        [Command]
        public void CmdCast(int spellSlotId)
        {
            if(!isServer)
                return;

            if (_state.CanCast && spellSlotId >= 0 && spellSlotId < _spellStates.Count)
            {
                var spellState = _spellStates[spellSlotId];
                if (spellState.CanCast)
                {
                    var spellHandlerId = spellState.CastServer(this, transform.position,
                        transform.TransformDirection(Vector3.forward));
                    RpcCast(spellSlotId, spellHandlerId, transform.position, transform.TransformDirection(Vector3.forward));
                }
            }
        }

        [ClientRpc]
        void RpcCast(int spellSlotId, int spellHandlerId, Vector3 position, Vector3 direction)
        {
            // Because this cast is already on server
            if (!isServer)
            {
                var spellState = _spellStates[spellSlotId];
                spellState.CastClient(spellHandlerId, this, position, direction);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Offset, StartRadius);
        }

        [Server]
        public void SendSpellEvent(SpellHandler handler, int eventId)
        {
            var spellSlotId = Spells.IndexOf(handler.Spell);
            Debug.LogFormat("Sending spell event handler.Spell.name:{0} eventId:{1} spellSlotId:{2}", handler.Spell.name, eventId, spellSlotId);
            RpcSpellEvent(spellSlotId, handler.Id, eventId);
        }

        [ClientRpc]
        void RpcSpellEvent(int spellSlotId, int handlerId, int eventId)
        {
            Debug.LogFormat("Received spell event handlerId:{0} eventId:{1} spellSlotId:{2}", handlerId, eventId, spellSlotId);
            var spell = _spellStates[spellSlotId];
            spell.SendEvent(handlerId, eventId);
        }

        public void DestroySpellHandler(SpellHandler spellHandler)
        {
            var spellState = _spellStates.FirstOrDefault(state => state.Spell == spellHandler.Spell);
            spellState.DestroyHandler(spellHandler.Id);
        }
    }
}
