using UnityEngine;
using System.Collections;

public class PlayerMoving : MonoBehaviour {

    /// <summary>
    /// Applied force multiplier
    /// </summary>
    public float ForceValue = 10f;
    public float MaximumSpeed = 0.05f;

    public float Speed = 10f;

    private Vector3 _previousPoint = new Vector3(0, 0, 0);
    private Vector3 _currentPoint = new Vector3(0, 0, 0);

    /// <summary>
    /// Ref to player's rigid body
    /// </summary>
    private Rigidbody _rigidBody;

    // Use this for initialization
    void Start () {
        _rigidBody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    void FixedUpdate()
    {
        _previousPoint = _currentPoint;
        _currentPoint = transform.position;
        Speed = GetSpeed();

        if (Speed >= MaximumSpeed)
            return;

        var speedVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        speedVector *= ForceValue * Time.deltaTime;
        _rigidBody.AddForce(speedVector, ForceMode.Impulse);
    }
    
    private float GetSpeed()
    {
        return (_currentPoint - _previousPoint).magnitude;
    }
}
