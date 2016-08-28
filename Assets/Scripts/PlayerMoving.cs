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

    public ParticleSystem BloodParticles;

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
    private ParticleSystem _bloodParticles;
    private CameraMovement _cameraMovement;

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
        _cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GameObject.Find("FootDust").GetComponent<ParticleSystem>();
        _bloodParticles = Instantiate(BloodParticles);
        _bloodParticles.Stop();
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

        if (_forceValue == ForceValueForIce)
        {
            var angle = Vector3.Angle(_rigidBody.velocity, _rigidBody.transform.forward) * 2 * Mathf.PI /360;
            var animationSpeed = 1.5f * velocityMagnitude * (Mathf.Abs(Mathf.Sin(angle * 2f)) + 0.5f);
            _childAnimator.SetFloat("Speed", animationSpeed);
        }
        else
        {
            _childAnimator.SetFloat("Speed", velocityMagnitude);
        }

        TryFloor();
    }

    /// <summary>
    /// Getting a punch from any other object
    /// </summary>
    /// <param name="direction"></param>
    public void GetPunched(Vector3 direction)
    {
        _rigidBody.AddForce(((direction + new Vector3(0f, 1f, 0f)).normalized) * PunchMultiplier, ForceMode.Impulse);
    }

    /// <summary>
    /// Actions made after player is killed
    /// </summary>
    public void InitiateDeath()
    {
        _bloodParticles.transform.position = transform.position + new Vector3(0f, 0.05f, 0f);
        _bloodParticles.Play();
        _lastPunchTime = Time.time - 2 * _punchCooldown;
        PlayerToSpawn();
    }

    public void TryFloor()
    {
        RaycastHit hit;
        var forward = transform.TransformDirection(new Vector3(0f, -1f, 0.05f));
        var initialPosition = transform.position;
        initialPosition.y = 0.5f;
        var ray = new Ray(initialPosition, forward);
        if (Physics.Raycast(ray, out hit, 1f))
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
        _cameraMovement.SetLastTrackedPosition(transform.position);
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

        if (col.gameObject.CompareTag(Tags.Respawn) && SpawnPoint != col.gameObject)
        {
            if(SpawnPoint != null)
                SpawnPoint.SendMessage("OnPlayerDeactivate");
            SpawnPoint = col.gameObject;
            col.gameObject.SendMessage("OnPlayerActivate");
            //col.gameObject.GetComponent<Animator>().SetTrigger("Rotate");
        }
    }

    void OnTriggerExit(Collider col)
    {
    }
}
