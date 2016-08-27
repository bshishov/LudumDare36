using UnityEngine;
using System.Collections;

public class PlayerMoving : MonoBehaviour {

    /// <summary>
    /// Applied force multiplier
    /// </summary>
    public float ForceValue = 10f;
    public float MaximumSpeed = 0.05f;
    public float RotationSpeed = 10f;

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
        var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

        var localTarget = transform.InverseTransformPoint(transform.localPosition + forceVector);
        var angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        var eulerAngleVelocity = new Vector3(0, angle, 0);
        var deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * RotationSpeed);
        _rigidBody.MoveRotation(_rigidBody.rotation * deltaRotation);
    }

    void FixedUpdate()
    {
        _previousPoint = _currentPoint;
        _currentPoint = transform.position;
        Speed = GetSpeed();

        if (Speed >= MaximumSpeed)
            return;

        var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        forceVector *= ForceValue * Time.deltaTime;
        _rigidBody.AddForce(forceVector, ForceMode.Impulse);
        
        var localTarget = transform.InverseTransformPoint(transform.localPosition + forceVector);
        var angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        var eulerAngleVelocity = new Vector3(0, angle, 0);
        var deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * RotationSpeed);
        _rigidBody.MoveRotation(_rigidBody.rotation * deltaRotation);
    }
    
    private float GetSpeed()
    {
        return (_currentPoint - _previousPoint).magnitude;
    }

    void OnCollisionEnter(Collision col)
    {
    }

    void OnCollisionExit(Collision col)
    {
    }
}
