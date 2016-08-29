using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour
{
    public float Delay;
	
	void Start () {
        Object.Destroy(gameObject, Delay);
    }
}
