using UnityEngine;
using System.Collections;

public class Poleaxe : MonoBehaviour {

    public AudioClip WhooshClip;
    private AudioSource _audioSource;

    // Use this for initialization
    void Start ()
    {
        _audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void PlaySound()
    {
        _audioSource.Play();
    }
}
