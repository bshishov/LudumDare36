using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMoving : NetworkBehaviour
{
    public bool IsAlive { get; private set; }

    private const float KillingFloorY = -2f;
    private const float PunchCooldown = 0.5f;

    public float DefaultForceValue = 70f;
    public float ForceValueForIce = 0.2f;
    public float ForceValueInAir = 35f;
    public float MaximumSpeed = 0.05f;
    public float RotationSpeed = 10f;
    public float PunchMultiplier = 25f;
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

    private float _startTime;
    private int _deathNumber = 0;

    [HideInInspector]
    public int DeathNumber {  get { return _deathNumber; } }

    void Start ()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
        _spawnPoint = GameObject.Find(SpawnPoint.MainSpawnName);
        _startTime = Time.time;
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
            
            var inputVector = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), 1f);

            if (inputVector.magnitude > 0.01f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputVector.normalized),
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

        if (isLocalPlayer && IsAlive)
        {
            var inputVector = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), 1f);

            //_rigidBody.MovePosition(transform.position + inputVector * Time.deltaTime * 1.9f);
            //_rigidBody.AddForce(inputVector * _forceValue * Time.deltaTime, ForceMode.Impulse);
            _rigidBody.AddForce(inputVector * _forceValue * Time.deltaTime * 2f, ForceMode.Impulse);
            //_rigidBody.AddForce(inputVector * Time.deltaTime * 20.7f, ForceMode.VelocityChange);
            //_childAnimator.SetFloat("Speed", inputVector.magnitude * 1.7f);

            // clamp horizontal velocity to maximum
            // only for a loval player (non-local handled over network transform)
            if (velocityMagnitude > MaximumSpeed)
            {
                var horizontalClamped = Vector3.ClampMagnitude(new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z), MaximumSpeed);
                _rigidBody.velocity = new Vector3(horizontalClamped.x, _rigidBody.velocity.y, horizontalClamped.z);
            }
        }

        // setting animation speed
        if (_forceValue == ForceValueForIce)
        {
            var angle = Vector3.Angle(_rigidBody.velocity, _rigidBody.transform.forward) * 2 * Mathf.PI /360;
            var animationSpeed = 1.5f * velocityMagnitude * (Mathf.Abs(Mathf.Sin(angle * 2f)) + 0.5f);
            //_childAnimator.SetFloat("Speed", animationSpeed);
        }
        else
        {
           // _childAnimator.SetFloat("Speed", velocityMagnitude);
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
        _rigidBody.AddForce((direction + new Vector3(0f, 0.5f, 0f)).normalized * PunchMultiplier, ForceMode.Impulse);
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
        _deathNumber++;
        IsAlive = false;
        Instantiate(BloodParticles, transform.position + new Vector3(0f, 0.05f, 0f), Quaternion.identity);

        //_lastDeathTime = Time.time;
        _lastPunchTime = Time.time - 2 * PunchCooldown;
        CmdPlayerToSpawn();
    }

    public void TryFloor()
    {
        const float rayEmitterHeight = 0.1f;
        const float rayZ = 0.25f;

        RaycastHit hit;
        var forward = transform.TransformDirection(new Vector3(0f, -1f, rayZ));
        var initialPosition = transform.position;
        initialPosition.y = rayEmitterHeight;
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
        else
        {
            _forceValue = ForceValueInAir;
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

    public void PlayerFinished()
    {
        var timeDelta = (int)Math.Round(Time.time - _startTime);
        var wholeMinutes = timeDelta / 60;
        var wholeSeconds = timeDelta % 60;

        var pauseMenuControler = GameObject.Find("PauseCanvas").GetComponent<PauseMenuController>();
        pauseMenuControler.GameOver = true;
        pauseMenuControler.PauseCanvas.SetActive(true);
        var pauseCanvasController = pauseMenuControler.PauseCanvas.GetComponent<PauseMenuButtons>();
        pauseCanvasController.PlayButton.SetActive(false);
        pauseCanvasController.GameOverPanel.SetActive(true);

        GameObject.Find("Time").GetComponent<Text>().text = string.Format("{0} m {1} s", wholeMinutes, wholeSeconds);
        GameObject.Find("Deaths").GetComponent<Text>().text = string.Format("{0} times", _deathNumber);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(Tags.MovingPlatform))
        {
            transform.parent = col.transform;
        }
        if (col.gameObject.CompareTag(Tags.Finish))
        {
            PlayerFinished();
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

#if DEBUG
    void OnGUI()
    {
        if (isLocalPlayer)
        {
            var inputVector =
                Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), 1f);
            GUI.TextField(new Rect(0, 0, 200, 20), string.Format("input: {0}", inputVector.ToString()));
            GUI.TextField(new Rect(0, 25, 200, 20), string.Format("velocity: {0}", _rigidBody.velocity.ToString()));
            GUI.TextField(new Rect(0, 50, 200, 20), string.Format("velocity: {0}", _rigidBody.velocity.magnitude));
        }
    }
#endif
}
