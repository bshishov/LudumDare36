using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public float RotationSpeed = 90f;
    public Vector3 RotationDirection = new Vector3(0f, 1f, 0f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(
            RotationDirection.x * RotationSpeed * Time.deltaTime, 
            RotationDirection.y * RotationSpeed * Time.deltaTime,
            RotationDirection.z * RotationSpeed * Time.deltaTime, 
            Space.World);
	}
}
