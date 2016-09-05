using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static string MainSpawnName = "MainSpawn";
    public static string PlayerActivateMessage = "OnPlayerActivate";
    public static string PlayerDeactivateMessage = "OnPlayerDeactivate";
    public static string PlayerRespawnMessage = "OnPlayerRespawn";

    private ParticleSystem _particleSystem;
    private Animator _bannerAnimator;
    private AudioSource _audioSource;

    void Start()
	{
	    _particleSystem = GetComponentInChildren<ParticleSystem>();
        _bannerAnimator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
	}

    public void OnPlayerRespawn()
    {
        if (!_particleSystem.isPlaying)
            _particleSystem.Play();

        if(_bannerAnimator != null)
            _bannerAnimator.SetTrigger("Respawn");
    }

    public void OnPlayerActivate()
    {
        if(!_particleSystem.isPlaying)
            _particleSystem.Play();

        if(_audioSource != null)
            _audioSource.Play();

        if (_bannerAnimator != null)
            _bannerAnimator.SetTrigger("Rotate");
    }

    public void OnPlayerDeactivate()
    {
        if (_particleSystem.isPlaying)
            _particleSystem.Stop();
    }
}
