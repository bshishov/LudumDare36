using System;
using Assets.Scripts.Gameplay.Player;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "Spells/Punch")]
    public class PunchSpell : SpellHandler
    {
        public const int PunchHitSuccess = 0;
        public const int PunchHitFailed = 1;

        public AudioClip PunchAudio;
        public Vector3 PunchForce = new Vector3(0, 10, 25);
        public float MaxPunchRadius = 0.5f;
    
        public override void Cast(Vector3 position, Vector3 direction)
        {
            Caster.GetComponentInChildren<Animator>().SetTrigger("Punch");

            if (Caster.isServer)
            {
                RaycastHit hit;
                var forward = Caster.transform.TransformDirection(Vector3.forward);
                var initialPosition = Caster.transform.position;
                initialPosition.y = 0.5f;
                var ray = new Ray(initialPosition, forward);
                if (Physics.Raycast(ray, out hit, MaxPunchRadius))
                {
                    if (hit.collider != null && hit.collider.gameObject.CompareTag(Tags.Player))
                    {
                        hit.collider.gameObject.GetComponent<PlayerMovement>().CmdPushPlayer(Caster.transform.TransformVector(PunchForce));
                        RaiseEvent(PunchHitSuccess);
                    }
                    else
                    {
                        RaiseEvent(PunchHitFailed);
                    }
                }
                else
                {
                    RaiseEvent(PunchHitFailed);
                }
            }
        }

        public override void OnSpellEvent(int spellEvent)
        {
            if (spellEvent == PunchHitSuccess)
            {
                Debug.Log("Punch hit success");
                Caster.GetComponent<AudioSource>().PlayOneShot(PunchAudio);
                this.Destroy();
            }

            if (spellEvent == PunchHitFailed)
            {
                Debug.Log("Punch hit failed");
                this.Destroy();
            }
        }
    }
}