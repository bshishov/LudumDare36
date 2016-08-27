using UnityEngine;
using System.Collections;

public class PlayerMoving : MonoBehaviour {

    public float SpeedVelue = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var x = Input.GetAxis("Horizontal") * SpeedVelue;
        var z = Input.GetAxis("Vertical") * SpeedVelue;
        var speedVector = new Vector3(x, 0f, z);
        transform.Translate(speedVector.normalized, Space.World);
    }
}
