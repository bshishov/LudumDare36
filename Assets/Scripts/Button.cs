using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Button : MonoBehaviour
{
    public GameObject[] Targets;
    public float PressDepth = 0.1f;
    public float PressingSpeed = 4f;

    private int _playersTriggered = 0;
    private float _currentState = 0f;
    private Vector3 _initialPosition;
    private Vector3 _pressedPosition;
    private AudioSource _audioSource;

    public bool IsActivated { get; private set; }

	void Start ()
	{
	    _initialPosition = transform.position;
	    _pressedPosition = transform.position + Vector3.down*PressDepth;
        IsActivated = false;

        _audioSource = GetComponent<AudioSource>();

        if (!GetComponent<Collider>().isTrigger)
            Debug.LogWarning("Button collider must be a trigger", this);
    }
	
	void Update ()
    {
	    if (IsActivated && _currentState < 1f)
	    {
	        _currentState += Time.deltaTime * PressingSpeed;
            transform.position = Vector3.Lerp(_initialPosition, _pressedPosition, _currentState);
        }

        if (!IsActivated && _currentState > 0f)
        {
            _currentState -= Time.deltaTime * PressingSpeed;
            transform.position = Vector3.Lerp(_initialPosition, _pressedPosition, _currentState);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.Player))
        {
            _playersTriggered++;

            if (!IsActivated && _playersTriggered == 1)
            {
                IsActivated = true;
                _audioSource.Play();
                foreach (var target in Targets)
                {
                    target.SendMessage(ActivatorProxy.ActivateEvent);
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.Player))
        {
            _playersTriggered--;

            if (IsActivated && _playersTriggered == 0)
            {
                IsActivated = false;
                foreach (var target in Targets)
                {
                    target.SendMessage(ActivatorProxy.DeActivateEvent);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var target in Targets)
        {
            if (target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }
    }
}
