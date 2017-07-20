using System;
using Assets.Scripts.Data;
using Assets.Scripts.Gameplay.Player;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class Bomb : MonoBehaviour
    {
        public GameObject DestroyEffectPrefab;
        public float BlastRadius = 2f;
        public float BlastPower = 1f;
        public BuffData ApplyBuff;
        public event Action OnCollide;

        private Collider _collider;

        void Start()
        {
            _collider = this.GetComponent<Collider>();
        }
	
        void OnCollisionEnter(Collision collision)
        {
            if (_collider.enabled)
            {
                if (OnCollide != null)
                    OnCollide();
                _collider.enabled = false;
            }
        }

        public void BlastAndDestory()
        {
            if (DestroyEffectPrefab != null)
            {
                GameObject.Instantiate(DestroyEffectPrefab, transform.position, Quaternion.identity);
            }
        
            var colliders = Physics.OverlapSphere(transform.position, 
                BlastRadius, 
                LayerMask.GetMask("Default"),
                QueryTriggerInteraction.Ignore);

            foreach (var cldr in colliders)
            {
                if (cldr is CapsuleCollider && cldr.gameObject.CompareTag(Tags.Player))
                {
                    if (BlastPower > 0)
                    {
                        var movement = cldr.gameObject.GetComponent<PlayerMovement>();
                        if (movement != null && movement.isServer)
                        {
                            var direction = cldr.transform.position - transform.position;
                            if (direction.y <= 0.1f)
                                direction.y = 0.1f;

                            movement.CmdPushPlayer(direction.normalized * BlastPower / (1f +direction.magnitude));
                        }
                    }

                    if (ApplyBuff != null)
                    {
                        var playerState = cldr.gameObject.GetComponent<PlayerState>();
                        if (playerState.isServer)
                            playerState.CmdApplyBuff(ApplyBuff.name);
                    }
                }
            }

            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, BlastRadius);
        }
    }
}
