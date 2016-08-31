using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ActivatorProxy))]
public class ActivatableSound : MonoBehaviour
{
    public AudioClip ActivateSound;
    public AudioClip DeactivateSound;

    private AudioSource _audioSource;
	
	void Start ()
	{
	    _audioSource = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
	
	}

    void OnProxyActivate()
    {
        if (ActivateSound != null)
        {
            _audioSource.clip = ActivateSound;
            _audioSource.Play();
        }
    }

    void OnProxyDeactivate()
    {
        if (DeactivateSound != null)
        {
            _audioSource.clip = DeactivateSound;
            _audioSource.Play();
        }
    }
}
