using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Button : MonoBehaviour
{
    public static string ActivateEvent = "OnButtonActivate";
    public static string DeActivateEvent = "OnButtonDeactivate";

    public GameObject Target;
    public float PressDepth = 0.1f;
    public float PressingSpeed = 4f;

    private float _currentState = 0f;
    private Vector3 _initialPosition;
    private Vector3 _pressedPosition;

    public bool IsActivated { get; private set; }

	void Start ()
	{
	    _initialPosition = transform.position;
	    _pressedPosition = transform.position + Vector3.down*PressDepth;
        IsActivated = false;

        if(!GetComponent<Collider>().isTrigger)
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
        Debug.Log("trigger enter");
        if (col.gameObject.CompareTag(Tags.Player))
        {
            if (!IsActivated)
            {
                IsActivated = true;
                if (Target != null)
                {
                    Target.SendMessage(ActivateEvent);
                    Debug.LogFormat("Pressed button {0}", gameObject.name);
                }
                else
                {
                    Debug.LogWarning("Trying to activate, but button target is not set", this);
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.Player))
        {
            if (IsActivated)
            {
                IsActivated = false;
                if (Target != null)
                {
                    Target.SendMessage(DeActivateEvent);
                    Debug.LogFormat("Unpressed button {0}", gameObject.name);
                }
                else
                {
                    Debug.LogWarning("Trying to deactivate, but button target is not set", this);
                }
            }
        }
    }
}
