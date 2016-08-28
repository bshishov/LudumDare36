using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public float RotationSpeed = 90f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(0f, RotationSpeed * Time.deltaTime, 0f, Space.World);
	}
}
