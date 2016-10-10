using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class PlayerPunch : NetworkBehaviour
{
    public AudioClip PushAudio;
    public const float PunchCooldown = 0.5f;
    public Vector3 PunchForce = new Vector3(0, 10, 25);

    private PlayerState _state;
    private Animator _animator;
    private AudioSource _audioSource;
    private float _lastPunchTime = 0f;

    // Use this for initialization
    void Start ()
    {
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _state = GetComponent<PlayerState>();
    }

    [Command]
    public void CmdPunch()
    {
        if(!isServer)
            return;

        if(!_state.IsAlive)
            return;

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
                    hit.collider.gameObject.GetComponent<PlayerMovement>().CmdPushPlayer(transform.TransformVector(PunchForce));
                    RpcPunchSuccess();
                }
            }
        }
    }

    [ClientRpc]
    void RpcPlayPunchAnimation()
    {
        _animator.SetTrigger("Punch");
    }

    [ClientRpc]
    void RpcPunchSuccess()
    {
        _audioSource.PlayOneShot(PushAudio, 0.8f);
    }
}
