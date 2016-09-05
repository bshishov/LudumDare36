using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMoving : NetworkBehaviour
{
    public bool IsAlive { get; private set; }

    private const float KillingFloorY = -2f;
    private const float PunchCooldown = 0.5f;

    public float DefaultForceValue = 70f;
    public float ForceValueForIce = 0.2f;
    public float MaximumSpeed = 0.05f;
    public float RotationSpeed = 10f;
    public float PunchMultiplier = 20f;
    public AudioClip PushAudio;
    public GameObject BloodParticles;
    
    private Animator _childAnimator;
    private ParticleSystem _dustParticleSystem;
    private CameraMovement _cameraMovement;
    private AudioSource _audioSource;
    private Rigidbody _rigidBody;
    private GameObject _spawnPoint;

    //private float _deathCooldown = 0.3f;
    private float _lastPunchTime = 0f;
    //private float _lastDeathTime = 0f;
    private float _forceValue;

    void Start ()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
        _spawnPoint = GameObject.Find(SpawnPoint.MainSpawnName);
    }

    public override void OnStartLocalPlayer()
    {
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
        _cameraMovement.SetTarget(gameObject);
        _forceValue = DefaultForceValue;
        CmdPlayerToSpawn();
    }
    
    void Update ()
    {
        if (isLocalPlayer && IsAlive)
        {
            if (transform.position.y <= KillingFloorY)
            {
                CmdInitiateDeath();
            }

            var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

            if (forceVector.magnitude > 0.01f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forceVector.normalized),
                    Time.deltaTime*RotationSpeed);
            }

            if (Input.GetButtonDown("Jump"))
            {
                CmdPunch();
            }
        }
    }

    void FixedUpdate()
    {
        var velocityMagnitude = new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z).magnitude;
        _childAnimator.SetFloat("Speed", velocityMagnitude);

        if (!_dustParticleSystem.isPlaying && velocityMagnitude > 0.1f)
        {
            _dustParticleSystem.Play();
        }
        else if (_dustParticleSystem.isPlaying && velocityMagnitude < 0.1f)
        {
            _dustParticleSystem.Stop();
        }

        //if (!isLocalPlayer)
        //    return;

        if (velocityMagnitude >= MaximumSpeed)
            return;

        if (isLocalPlayer && IsAlive)
        {
            var forceVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            forceVector *= _forceValue*Time.deltaTime;
            _rigidBody.AddForce(forceVector, ForceMode.Impulse);
        }

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
    /// Punch command
    /// </summary>
    [Command]
    public void CmdPunch()
    {
        if (Time.time - _lastPunchTime >= PunchCooldown)
        {
            _lastPunchTime = Time.time;
            RpcPlayPunchAnimation(); // Play animation on this player

            RaycastHit hit;
            var forward = transform.TransformDirection(Vector3.forward);
            var initialPosition = transform.position;
            initialPosition.y = 0.5f;
            var ray = new Ray(initialPosition, forward);
            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag(Tags.Player))
                {
                    hit.collider.gameObject.GetComponent<PlayerMoving>().RpcPushPlayer(forward);
                }
            }
        }
    }

    [ClientRpc]
    public void RpcPlayPunchAnimation()
    {
        _childAnimator.SetTrigger("Punch");
    }

    [ClientRpc]
    public void RpcPushPlayer(Vector3 direction)
    {
        _rigidBody.AddForce(((direction + new Vector3(0f, 1f, 0f)).normalized) * PunchMultiplier, ForceMode.Impulse);
        _audioSource.PlayOneShot(PushAudio, 0.8f);
    }

    [Command]
    public void CmdInitiateDeath()
    {
        RpcInitiateDeath();
    }

    [ClientRpc]
    public void RpcInitiateDeath()
    {
        IsAlive = false;
        Instantiate(BloodParticles, transform.position + new Vector3(0f, 0.05f, 0f), Quaternion.identity);

        //_lastDeathTime = Time.time;
        _lastPunchTime = Time.time - 2 * PunchCooldown;
        CmdPlayerToSpawn();
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

    [Command]
    public void CmdPlayerToSpawn()
    {
        RpcPlayerToSpawn();
    }

    [ClientRpc]
    public void RpcPlayerToSpawn()
    {
        if (isLocalPlayer)
        {
            transform.position = _spawnPoint.transform.position;
            _spawnPoint.SendMessage(SpawnPoint.PlayerRespawnMessage);
        }
        IsAlive = true;
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
            // If leaving same object
            if(transform.parent == col.gameObject.transform)
                transform.parent = null;
        }
    }

    void OnCollisionStay(Collision col)
    {
    }

    void OnTriggerEnter(Collider col)
    {
        if (isServer)
        {
            if (col.gameObject.CompareTag(Tags.Killer))
            {
                CmdInitiateDeath();
            }
        }

        if (isLocalPlayer)
        {
            if (col.gameObject.CompareTag(Tags.Respawn) && 
                _spawnPoint != col.gameObject && 
                col.gameObject.name != SpawnPoint.MainSpawnName)
            {
                if (_spawnPoint != null)
                    _spawnPoint.SendMessage(SpawnPoint.PlayerDeactivateMessage);
                _spawnPoint = col.gameObject;
                _spawnPoint.SendMessage(SpawnPoint.PlayerActivateMessage);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
    }
}
