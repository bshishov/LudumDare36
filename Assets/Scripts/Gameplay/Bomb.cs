using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class Bomb : MonoBehaviour
{
    public GameObject DestroyEffectPrefab;
    public float BlastRadius = 2f;
    public float BlastPower = 1f;
    public BuffData ApplyBuff;
    
	
    void OnCollisionEnter(Collision collision)
    {
        if (DestroyEffectPrefab != null)
        {
            GameObject.Instantiate(DestroyEffectPrefab, transform.position, Quaternion.identity);
        }
        
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            // TODO: ADD SOMETHING ??
        }

        var colliders = Physics.OverlapSphere(transform.position, BlastRadius);
        foreach (var cldr in colliders)
        {
            if (cldr.gameObject.CompareTag(Tags.Player))
            {
                if (BlastPower > 0)
                {
                    var movement = cldr.gameObject.GetComponent<PlayerMovement>();
                    if (movement != null && movement.isServer)
                    {
                        var direction = (cldr.transform.position - transform.position).normalized;
                        if (direction.y <= 0.1f)
                            direction.y = 0.1f;

                        movement.CmdPushPlayer(direction*BlastPower);
                    }
                }

                if (ApplyBuff != null)
                {
                    var playerState = cldr.gameObject.GetComponent<PlayerState>();
                    if(playerState.isServer)
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
