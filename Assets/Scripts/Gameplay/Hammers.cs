using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Hammers : MonoBehaviour
{
    public AudioClip HitClip;
    public AudioClip ChargeClip;

    private AudioSource _audioSource;
    private Animator _animator;

    void Start ()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        var networkAnimator = GetComponent<NetworkAnimator>();

        //na.SetParameterAutoSend();
    }
	
	void Update ()
    {
	
	}

    void StartHitSound()
    {
        _audioSource.clip = HitClip;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    void StartChargeSound()
    {
        _audioSource.clip = ChargeClip;
        //_audioSource.loop = true;
        _audioSource.Play();
    }

    void StopCurrentSound()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
