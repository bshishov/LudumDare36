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

    void Start ()
    {
        _audioSource = GetComponent<AudioSource>();
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
        _audioSource.Play();
    }

    void StopCurrentSound()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
