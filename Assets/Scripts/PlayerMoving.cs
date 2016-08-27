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
    
    private Animator _childAnimator;
    private ParticleSystem _dustParticleSystem;

    /// <summary>
    /// Ref to player's rigid body
    /// </summary>
    private Rigidbody _rigidBody;

    private float _punchCooldown = 0.5f;
    private float _lastPunchTime = 0f;

    void Start ()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        _dustParticleSystem.Stop();
    }
	
	// Update is called once per frame
	void Update ()
    {
        var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

        if (forceVector.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forceVector.normalized),
                Time.deltaTime * RotationSpeed);
        }

        if (Input.GetButtonDown("Jump"))
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
        var velocityMagnitude = Speed = _rigidBody.velocity.magnitude;
        _childAnimator.SetFloat("Speed", velocityMagnitude);

        if (!_dustParticleSystem.isPlaying && velocityMagnitude > 0.01f)
        {
            _dustParticleSystem.Play();
        }
        else if(_dustParticleSystem.isPlaying && velocityMagnitude < 0.01f)
        {
            _dustParticleSystem.Stop();
        }

        if (velocityMagnitude >= MaximumSpeed)
            return;

        var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        forceVector *= ForceValue * Time.deltaTime;
        _rigidBody.AddForce(forceVector, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(Tags.MovingPlatform))
        {
            transform.parent = col.transform;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag(Tags.MovingPlatform))
        {
            transform.parent = null;
        }
    }

    void OnCollisionStay(Collision col)
    {
        /*
        if (col.gameObject.CompareTag(Tags.MovingPlatform))
        {
            transform.parent = col.transform;
        }
        else
        {
            transform.parent = null;
        }*/
    }
}
