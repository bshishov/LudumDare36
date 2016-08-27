using UnityEngine;
using System.Collections;

public class TimedActivator : MonoBehaviour
{
    public float TimeOffset = 0f;
    public float ActivateDelay = 1f;
    public float DeactivateDelay = 1f;

    public GameObject[] Targets;
    public bool Loop = true;


    private float _currentTime = 0f;
    private bool _activated = false;

    void Start ()
    {
        _currentTime = TimeOffset;
    }
	
	void Update ()
	{
	    _currentTime += Time.deltaTime;

	    if (!_activated && _currentTime > ActivateDelay && _currentTime < ActivateDelay + DeactivateDelay)
	    {
	        _activated = true;
            Emit(ActivatorProxy.ActivateEvent);
	    }

	    if (_activated && _currentTime > ActivateDelay + DeactivateDelay)
	    {
	        _activated = false;
            Emit(ActivatorProxy.DeActivateEvent);

            if (Loop)
	            _currentTime = 0f;
	    }
	}

    void Emit(string methodName)
    {
        foreach (var target in Targets)
        {
            target.SendMessage(methodName);
        }
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
