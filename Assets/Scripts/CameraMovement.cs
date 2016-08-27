using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public GameObject TrackedObject;
    public float SmoothTime = 2f;
    public float LookAhead = 2f;

    private Vector3 _velocity;
    private Vector3 _offset;
    private Vector3 _lastTrackedObjectPosition;


    void Start ()
	{
	    _offset = transform.position - TrackedObject.transform.position;
        _lastTrackedObjectPosition = TrackedObject.transform.position;
	}
	
	void Update ()
	{
	    var trackedObjectVelocity = TrackedObject.transform.position - _lastTrackedObjectPosition;
	    _lastTrackedObjectPosition = TrackedObject.transform.position;

        transform.position = Vector3.SmoothDamp(transform.position, 
            TrackedObject.transform.position + _offset + trackedObjectVelocity * LookAhead, 
            ref _velocity, 
            SmoothTime);
	}

    void OnDrawGizmosSelected()
    {
        if (TrackedObject != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, TrackedObject.transform.position);
        }
    }
}
