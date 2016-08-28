using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoving : NetworkBehaviour
{

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
    
    private Animator _childAnimator;
    private ParticleSystem _dustParticleSystem;
    private ParticleSystem _bloodParticles;
    private CameraMovement _cameraMovement;

    /// <summary>
    /// Ref to player's rigid body
    /// </summary>
    private Rigidbody _rigidBody;
    private GameObject _spawnPoint;

    private float _punchCooldown = 0.5f;
    private float _lastPunchTime = 0f;

    [SerializeField]
    private float _forceValue;

    void Start ()
    {
        // prepare components
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        _spawnPoint = GameObject.Find("MainSpawn");
        _bloodParticles = Instantiate(BloodParticles);

        if (!isLocalPlayer)
            return;

        // initialization
        _cameraMovement.SetTarget(gameObject);
        PlayerToSpawn();
        _forceValue = DefaultForceValue;
    }

    public override void OnStartLocalPlayer()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
            return;

        if (transform.position.y <= -2f)
            InitiateDeath();

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
                if (Physics.Raycast(ray, out hit, 0.5f))
                    if (hit.collider != null && hit.collider.gameObject.CompareTag(Tags.Player))
                        CmdPunch(hit.collider.gameObject, forward);
            }
        }
    }

    void FixedUpdate()
    {
        var velocityMagnitude = Speed = _rigidBody.velocity.magnitude;
        _childAnimator.SetFloat("Speed", velocityMagnitude);

        if (!_dustParticleSystem.isPlaying && velocityMagnitude > 0.1f)
        {
            _dustParticleSystem.Play();
        }
        else if (_dustParticleSystem.isPlaying && velocityMagnitude < 0.1f)
        {
            _dustParticleSystem.Stop();
        }

        if (!isLocalPlayer)
            return;

        if (velocityMagnitude >= MaximumSpeed)
            return;

        var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        forceVector *= _forceValue * Time.deltaTime;
        _rigidBody.AddForce(forceVector, ForceMode.Impulse);

        // setting animation speed
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
    [Command]
    public void CmdPunch(GameObject target, Vector3 direction)
    {
        if (!isServer)
            return;

        target.GetComponent<PlayerMoving>().RpcPushPlayer(direction);
    }

    [ClientRpc]
    public void RpcPushPlayer(Vector3 direction)
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
        transform.position = _spawnPoint.transform.position;
        _cameraMovement.SetLastTrackedPosition(transform.position);

        _spawnPoint.SendMessage(SpawnPoint.PlayerRespawnMessage);
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

        if (col.gameObject.CompareTag(Tags.Respawn) && _spawnPoint != col.gameObject)
        {
            if (!isLocalPlayer)
                return;

            if(_spawnPoint != null)
                _spawnPoint.SendMessage(SpawnPoint.PlayerDeactivateMessage);
            _spawnPoint = col.gameObject;
            _spawnPoint.SendMessage(SpawnPoint.PlayerActivateMessage);
        }
    }

    void OnTriggerExit(Collider col)
    {
    }
}
