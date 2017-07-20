using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Gameplay.Player
{
    public class PlayerState : NetworkBehaviour
    {
        public class BuffInstance
        {
            public float EndTime;
            public string Name;


            public BuffData Data { get; private set; }

            public BuffInstance(BuffData data)
            {
                Data = data;
                Name = data.name;
            }

            public void OnEnable(PlayerState state)
            {
                EndTime = Time.time + Data.Duration;
                if (Data.EffectPrefab != null)
                {
                    var effect = Instantiate(Data.EffectPrefab, state.transform, false) as GameObject;
                    Destroy(effect, Data.Duration);
                }
            }

            public void OnAffect(PlayerState state)
            {
                state._canRotate = Data.CanRotate;
                state._canWalk = Data.CanWalk;
                state._canCast = Data.CanCast;
                state.MovementInversed = Data.InverseMovement;
            }
   
            public void OnDisable(PlayerState state)
            {
            }

            public void Refresh()
            {
                EndTime = Time.time + Data.Duration;
            }
        }


        public float DeathCooldown = 2f;
        public GameObject DeathEffect;

        public bool IsAlive { get; private set; }
        public bool CanWalk { get { return IsAlive && _canWalk; } }
        public bool CanRotate { get { return IsAlive && _canRotate; } }
        public bool CanCast { get { return IsAlive && _canCast; } }
        public bool MovementInversed { get; private set; }
        public float SpeedModifier { get; private set; }

        private bool _canWalk;
        private bool _canRotate;
        private bool _canCast;
        private bool _respawnRequested;
        private float _respawnTime;
        private readonly List<BuffInstance> _buffs = new List<BuffInstance>();
        private BuffData[] _availableBuffs;

        public float StartTime { get; private set; }
        public int DeathsCount { get; private set; }

        void Start ()
        {
            _availableBuffs = Resources.FindObjectsOfTypeAll<BuffData>();
            SpeedModifier = 1.0f;
            _canWalk = true;
            _canRotate = true;
            _canCast = true;
            MovementInversed = false;
            IsAlive = true;
            StartTime = Time.time;
        }

        void FixedUpdate ()
        {
            if (isLocalPlayer)
            {
                if (_respawnRequested && Time.time > _respawnTime)
                    CmdRespawn();
            }

            // Proceed buffs
            var buffsNeedToBeUpdated = false;
            foreach (var buff in _buffs)
            {
                if (Time.time > buff.EndTime)
                {
                    buff.OnDisable(this);
                    buffsNeedToBeUpdated = true;
                }
            }
            _buffs.RemoveAll(buff => Time.time > buff.EndTime);

            if(buffsNeedToBeUpdated)
                BuffsUpdated();
        }

        [Command]
        public void CmdApplyBuff(string buffname)
        {
            if(!isServer)
                return;
        
            RpcApplyBuff(buffname);
        }

        void BuffsUpdated()
        {
            // Reset state
            _canWalk = true;
            _canRotate = true;
            _canCast = true;
            MovementInversed = false;
            SpeedModifier = 1.0f;

            // Re-apply buffs
            foreach (var buffInstance in _buffs)
            {
                buffInstance.OnAffect(this);
            }
        }

        [ClientRpc]
        void RpcApplyBuff(string buffname)
        {
            var buff = _availableBuffs.FirstOrDefault(b => b.name.Equals(buffname));
            if(buff == null)
                Debug.LogWarning(String.Format("Can't find buff {0}", buffname));

            var existed = _buffs.FirstOrDefault(b => b.Name.Equals(buffname));
            if (existed != null && !existed.Data.Stacks)
            {
                existed.Refresh();
            }
            else
            {
                var inst = new BuffInstance(buff);
                inst.OnEnable(this);
                _buffs.Add(inst);
                BuffsUpdated();
            }
        }

        [Command]
        public void CmdDeath()
        {   
            if(!isServer)
                return;

            if (!IsAlive)
                return;
        
            RpcDeath();
        }

        [Command]
        public void CmdRespawn()
        {
            if (!isServer)
                return;

            if (IsAlive)
                return;

            RpcRespawn();
        }
    
        [ClientRpc]
        void RpcDeath()
        {
            IsAlive = false;
            _respawnRequested = true;
            _respawnTime = Time.time + DeathCooldown;
            if (DeathEffect != null)
                GameObject.Instantiate(DeathEffect, transform.position + DeathEffect.transform.position, DeathEffect.transform.rotation);
            DeathsCount++;

            // disable all children objects
            for (var i = 0; i < gameObject.transform.childCount; i++)
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
        
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                GetComponent<PlayerMovement>().CmdTeleportToSpawn();
            }
        }

        [Command]
        public void CmdFinish()
        {
            if(!isServer)
                return;

            RpcPlayerFinished();
        }
    
        [ClientRpc]
        void RpcPlayerFinished()
        {
            // Disable the player
            _canWalk = false;
            _canRotate = false;
            _canCast = false;


            if (isLocalPlayer)
            {
                var timeDelta = (int) Math.Round(Time.time - StartTime);
                var wholeMinutes = timeDelta/60;
                var wholeSeconds = timeDelta%60;

                var uiManager = GameObject.FindObjectOfType<GameplayUIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowPanel(GameplayUIManager.UIPanel.GameOver);

                    //gameOverData.TimeText.text = string.Format("{0} m {1} s", wholeMinutes, wholeSeconds);
                    //gameOverData.DeathsText.text = string.Format("{0} times", DeathsCount);
                }
            }
        }

        public void OnSpawned()
        {
            IsAlive = true;
            _respawnRequested = false;

            // enable all children objects
            for (var i = 0; i < gameObject.transform.childCount; i++)
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
