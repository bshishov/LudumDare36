using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class Bomb : NetworkBehaviour
{
    public GameObject DestroyEffectPrefab;
	
	void Start ()
    {
	}	
	
	void Update ()
    {
	}

    void OnCollisionEnter(Collision collision)
    {
        if (DestroyEffectPrefab != null)
        {
            GameObject.Instantiate(DestroyEffectPrefab, transform.position, Quaternion.identity);
        }

        if (collision.gameObject.CompareTag(Tags.Player))
        {
            Debug.Log("Bomb collision with player");
        }

        Destroy(gameObject);
    }
}
