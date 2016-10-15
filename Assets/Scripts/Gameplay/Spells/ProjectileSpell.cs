using System;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Spells
{
    [Serializable]
    [CreateAssetMenu(fileName = "Spells/Projectile")]
    public class ProjectileSpell : SpellHandler
    {
        public const int ProjectileCollideEvent = 0;

        public Vector3 ProjectileForce = new Vector3(0, 8f, 15f);
        public GameObject ProjectilePrefab;
        
        private Bomb _bomb;

        public override void Cast(Vector3 position, Vector3 direction)
        {
            var spellInstance = GameObject.Instantiate(ProjectilePrefab, position + direction * Caster.StartRadius + Caster.Offset, Quaternion.identity) as GameObject;
            Debug.Assert(spellInstance != null, "spellInstance is null!!");
            
            _bomb = spellInstance.GetComponent<Bomb>();
            if (Caster.isServer)
            {
                _bomb.OnCollide += () =>
                {
                    RaiseEvent(ProjectileCollideEvent);
                };
            }

            var rigidBody = spellInstance.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.AddForce(Quaternion.FromToRotation(Vector3.forward, direction) * ProjectileForce, ForceMode.Impulse);
            }
        }

        public override void OnSpellEvent(int spellEvent)
        {
            if (spellEvent == ProjectileCollideEvent)
            {
                _bomb.BlastAndDestory();
                this.Destroy();
            }
        }

        public override void OnDestroy()
        {
            // No need to destroy a destroyed bomb
            //Destroy(_bomb.gameObject);
        }
    }
}