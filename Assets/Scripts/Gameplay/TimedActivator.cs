using UnityEngine;
using System.Collections;

public class TimedActivator : MonoBehaviour
{
    public float TimeOffset = 0f;
    public float ActivateAtTime = 1f;
    public float DeactivateAtTime = 2f;
    public float TotalDuration = 4f;

    public GameObject[] Targets;
    public bool Loop = true;
    public bool SelfTarget = false;

    private float _currentTime = 0f;
    private bool _activated = false;

    void Start ()
    {
        _currentTime = TimeOffset;
        if (DeactivateAtTime < ActivateAtTime)
            Debug.LogWarning("DeactivateAtTime time must be more than ActivateAtTime");

        if (DeactivateAtTime > TotalDuration)
            Debug.LogWarning("Duration must be more than DeactivateAtTime");
    }
	
	void Update ()
	{
	    _currentTime += Time.deltaTime;

	    if (!_activated && _currentTime > ActivateAtTime && _currentTime < DeactivateAtTime)
	    {
	        _activated = true;
            Emit(ActivatorProxy.ActivateEvent);
	    }

	    if (_activated && _currentTime > DeactivateAtTime)
	    {
	        _activated = false;
            Emit(ActivatorProxy.DeActivateEvent);
	    }

	    if (_currentTime > TotalDuration && Loop)
	        _currentTime = 0;
	}

    void Emit(string methodName)
    {
        foreach (var target in Targets)
        {
            if(target != null)
                target.SendMessage(methodName);
        }

        if(SelfTarget)
            gameObject.SendMessage(methodName);
    }

    void OnDrawGizmosSelected()
    {
        foreach (var target in Targets)
        {
            if (target != null && target != gameObject)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }
    }
}
