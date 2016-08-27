using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoving : MonoBehaviour {

    /// <summary>
    /// Applied force multiplier
    /// </summary>
    public float ForceValue = 10f;
    public float MaximumSpeed = 0.05f;
    public float RotationSpeed = 10f;
    public float Speed;

    private Vector3 _previousPoint = new Vector3(0, 0, 0);
    private Vector3 _currentPoint = new Vector3(0, 0, 0);

    private Animator _childAnimator;

    /// <summary>
    /// Ref to player's rigid body
    /// </summary>
    private Rigidbody _rigidBody;

    private float _punchCooldown = 0.5f;
    private float _lastPunchTime = 0f;

    // Use this for initialization
    void Start () {
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
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

        if (Input.GetAxis("Jump") != 0)
        {
            if (Time.time - _lastPunchTime >= _punchCooldown)
            {
                _lastPunchTime = Time.time;
                _childAnimator.SetTrigger("Punch");
            }
        }
    }

    void FixedUpdate()
    {
        _previousPoint = _currentPoint;
        _currentPoint = transform.position;
        var velocityMagnitude = Speed = _rigidBody.velocity.magnitude;
        _childAnimator.SetFloat("Speed", velocityMagnitude);

        if (velocityMagnitude >= MaximumSpeed)
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

    void OnCollisionEnter(Collision col)
    {
    }

    void OnCollisionExit(Collision col)
    {
    }
}
