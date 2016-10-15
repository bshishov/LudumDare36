using System;
using Assets.Scripts.Gameplay.Player;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class SpellHandler : ScriptableObject
    {
        /// <summary>
        /// Spell handler Id in spellstate
        /// </summary>
        [NonSerialized] public int Id;

        /// <summary>
        /// Spell information
        /// </summary>
        [NonSerialized] public SpellData Spell;

        /// <summary>
        /// Caster component.
        /// </summary>
        [NonSerialized] public PlayerSpellCaster Caster;

        /// <summary>
        /// This method is called when player casts a spell
        /// It is invoked both at the server and the client at the same time
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public virtual void Cast(Vector3 position, Vector3 direction)
        {
        }

        /// <summary>
        /// This method is called when the spell handles its own event
        /// Like bomb explosion
        /// </summary>
        /// <param name="spellEvent"></param>
        public virtual void OnSpellEvent(int spellEvent)
        {
        }

        /// <summary>
        /// Called when handler is about to be removed from memory
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// Raises an events which will be syncronized over network
        /// </summary>
        /// <param name="eventId"></param>
        protected void RaiseEvent(int eventId)
        {
            Caster.SendSpellEvent(this, eventId);
        }

        /// <summary>
        /// Queue a self destruction of spell handler
        /// </summary>
        protected void Destroy()
        {
            Caster.DestroySpellHandler(this);
        }
    }
}