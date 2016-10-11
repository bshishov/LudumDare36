using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : NetworkBehaviour
{
    private const float KillingFloorY = -2f;

    public float DefaultForceValue = 70f;
    public float ForceValueForIce = 0.2f;
    public float ForceValueInAir = 35f;
    public float MaximumSpeed = 0.05f;
    public float RotationSpeed = 10f;
    
    private Animator _childAnimator;
    private ParticleSystem _dustParticleSystem;
    private CameraMovement _cameraMovement;
    private Rigidbody _rigidBody;
    private GameObject _spawnPoint;
    private PlayerState _state;
    private float _forceValue;

    void Start ()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _childAnimator = GetComponentInChildren<Animator>();
        _dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        _spawnPoint = GameObject.Find(SpawnPoint.MainSpawnName);
        _state = GetComponent<PlayerState>();
    }

    public override void OnStartLocalPlayer()
    {
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
        _cameraMovement.SetTarget(gameObject);
        _forceValue = DefaultForceValue;
        CmdTeleportToSpawn();
    }

    public void InputMovement(float x, float z)
    {
        // Only local player's input is proceeded
        if (!isLocalPlayer)
            return;

        var inputVector = Vector3.ClampMagnitude(new Vector3(x, 0f, z), 1f);
        if (_state.MovementInversed)
            inputVector *= -1f;

        if (_state.CanRotate && inputVector.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(inputVector.normalized),
                Time.deltaTime * RotationSpeed);
        }

        var velocityMagnitude = new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z).magnitude;
        if (velocityMagnitude > MaximumSpeed)
        {
            inputVector *= velocityMagnitude/( MaximumSpeed * 100f);
        }

        if (_state.CanWalk)
            _rigidBody.AddForce(inputVector * _forceValue * Time.deltaTime * 2f * _state.SpeedModifier, ForceMode.Impulse);
        
        /*
        if (inputVector.magnitude > 0.1f)
        {// Clamp velocity
            
            if (velocityMagnitude > MaximumSpeed)
            {
                var horizontalClamped =
                    Vector3.ClampMagnitude(new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z), MaximumSpeed);
                _rigidBody.velocity = new Vector3(horizontalClamped.x, _rigidBody.velocity.y, horizontalClamped.z);
            }
        }*/
    }


    void FixedUpdate()
    {
        if(!_state.IsAlive)
            return;

        if(isServer)
        { 
            if (transform.position.y <= KillingFloorY)
            {
                _state.CmdDeath();
                return;
            }
        }

        var velocityMagnitude = new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z).magnitude;
        _childAnimator.SetFloat("Speed", velocityMagnitude);

        // Control the dust
        if (!_dustParticleSystem.isPlaying && velocityMagnitude > 0.1f)
        {
            _dustParticleSystem.Play();
        }
        else if (_dustParticleSystem.isPlaying && velocityMagnitude < 0.1f)
        {
            _dustParticleSystem.Stop();
        }

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
    public void CmdPushPlayer(Vector3 vector)
    {
        if(!isServer)
            return;

        if(!_state.IsAlive)
            return;

        RpcPushPlayer(vector);
    }


    [ClientRpc]
    void RpcPushPlayer(Vector3 vector)
    {
        _rigidBody.AddForce(vector, ForceMode.Impulse);
    }

    [Command]
    public void CmdTeleportToSpawn()
    {
        if(!isServer)
            return;

        RpcAfterTeleportToSpawn();
    }

    [ClientRpc]
    void RpcAfterTeleportToSpawn()
    {
        if (isLocalPlayer)
        {
            transform.position = _spawnPoint.transform.position;
            _spawnPoint.SendMessage(SpawnPoint.PlayerRespawnMessage);
        }

        _state.OnSpawned();
    }

    void OnCollisionEnter(Collision col)
    {
        if (isServer)
        {
            if (col.gameObject.CompareTag(Tags.Finish))
            {
                _state.CmdFinish();
            }
        }

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
    

    void OnTriggerEnter(Collider col)
    {
        if (isServer)
        {
            if (col.gameObject.CompareTag(Tags.Killer))
            {
                _state.CmdDeath();
            }
        }

        if (isLocalPlayer && _state.IsAlive)
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
