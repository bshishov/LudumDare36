using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerBombThrower : NetworkBehaviour
{
    public GameObject BombPrefab;
    public Vector3 Force = new Vector3(0, 0.1f, 1f);
    public Vector3 Offset = new Vector3();
    public float StartRadius;

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Throw();
        }
    }

    void Throw()
    {
        if (BombPrefab == null)
        {
            Debug.LogWarning("Bomb prefab is empty");
            return;
        }

        var direction = transform.TransformDirection(Vector3.forward);
        var bomb = GameObject.Instantiate(BombPrefab, transform.position + direction * StartRadius + Offset, Quaternion.identity) as GameObject;
        var rigidBody = bomb.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.AddForce(transform.TransformVector(Force), ForceMode.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Offset, StartRadius);
    }
}
