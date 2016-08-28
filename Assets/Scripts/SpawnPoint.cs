using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static string PlayerActivateMessageKey = "OnPlayerActivate";
    public static string PlayerDeactivateMessageKey = "OnPlayerDeactivate";
    public static string PlayerRespawnMessageKey = "OnPlayerRespawn";

    private ParticleSystem _particleSystem;
    private Animator _bannerAnimator;

    void Start()
	{
	    _particleSystem = GetComponentInChildren<ParticleSystem>();
        _bannerAnimator = GetComponentInChildren<Animator>();
        _particleSystem.Stop();
	}

    public void OnPlayerRespawn()
    {
        _bannerAnimator.SetTrigger("Rotate");
    }

    public void OnPlayerActivate()
    {
        if(!_particleSystem.isPlaying)
            _particleSystem.Play();
        _bannerAnimator.SetTrigger("Rotate");
    }

    public void OnPlayerDeactivate()
    {
        if (_particleSystem.isPlaying)
            _particleSystem.Stop();
    }
}
