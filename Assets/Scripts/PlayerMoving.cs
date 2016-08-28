using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoving : MonoBehaviour {

    /// <summary>
    /// Applied force multiplier
    /// </summary>
    public float DefaultForceValue = 70f;
    public float ForceValueForIce = 0.2f;
    public float MaximumSpeed = 0.05f;
    public float RotationSpeed = 10f;
    public float Speed;
    public float PunchMultiplier = 20f;

    /// <summary>
    /// Setting the spawn point of player
    /// </summary>
    public GameObject SpawnPoint;

    /// <summary>
    /// Temporary flag for controlling only one player object on the scene
    /// </summary>
    public bool MainPlayer = false;
    
    private Animator _childAnimator;
    private ParticleSystem _dustParticleSystem;

    /// <summary>
    /// Ref to player's rigid body
    /// </summary>
    private Rigidbody _rigidBody;

    private float _punchCooldown = 0.5f;
    private float _lastPunchTime = 0f;

    [SerializeField]
    private float _forceValue;

    void Start ()
    {
        // prepare components
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        _dustParticleSystem.Stop();

        // initialization
        PlayerToSpawn();
        _forceValue = DefaultForceValue;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.y <= -2f)
            InitiateDeath();

        if (!MainPlayer) return;

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

                RaycastHit hit;
                var forward = transform.TransformDirection(Vector3.forward);
                var initialPosition = transform.position;
                initialPosition.y = 0.5f;
                var ray = new Ray(initialPosition, forward);
                if (Physics.Raycast(ray, out hit, 0.3f))
                    if (hit.collider != null && hit.collider.gameObject.CompareTag(Tags.Player))
                        hit.collider.gameObject.GetComponent<PlayerMoving>().GetPunched(forward);
            }
        }
    }

    void FixedUpdate()
    {
        if (!MainPlayer) return;

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
        forceVector *= _forceValue * Time.deltaTime;
        _rigidBody.AddForce(forceVector, ForceMode.Impulse);

        TryFloor();
    }

    /// <summary>
    /// Getting a punch from any other object
    /// </summary>
    /// <param name="direction"></param>
    public void GetPunched(Vector3 direction)
    {
        _rigidBody.AddForce(direction.normalized * PunchMultiplier, ForceMode.Impulse);
    }

    /// <summary>
    /// Actions made after player is killed
    /// </summary>
    public void InitiateDeath()
    {
        _lastPunchTime = Time.time - 2 * _punchCooldown;
        PlayerToSpawn();
    }

    public void TryFloor()
    {
        RaycastHit hit;
        var forward = transform.TransformDirection(Vector3.down);
        var initialPosition = transform.position;
        var ray = new Ray(initialPosition, forward);
        if (Physics.Raycast(ray, out hit, 0.3f))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag(Tags.SlipperySurface))
            {
                _forceValue = ForceValueForIce;
            }
            else
            {
                _forceValue = DefaultForceValue;
            }
        }
    }

    public void PlayerToSpawn()
    {
        transform.position = SpawnPoint.transform.position;
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
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.Killer))
        {
            InitiateDeath();
        }

        if (col.gameObject.CompareTag(Tags.Respawn))
        {
            SpawnPoint = col.gameObject;
        }
    }

    void OnTriggerExit(Collider col)
    {
    }
}
