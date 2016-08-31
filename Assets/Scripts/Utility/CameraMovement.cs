using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject TrackedObject;
    public float SmoothTime = 2f;
    public float LookAhead = 2f;
    public Vector3 Offset = new Vector3(0f, 4f, -1f);

    private Vector3 _velocity;
    private Vector3 _lastTrackedObjectPosition;

    void Start ()
    {
	}
	
	void Update ()
	{
        if (!TrackedObject)
            return;

        var trackedObjectVelocity = TrackedObject.transform.position - _lastTrackedObjectPosition;
	    _lastTrackedObjectPosition = TrackedObject.transform.position;

        transform.position = Vector3.SmoothDamp(transform.position, 
            TrackedObject.transform.position + Offset + trackedObjectVelocity * LookAhead, 
            ref _velocity, 
            SmoothTime);
	}

    public void SetTarget(GameObject target)
    {
        TrackedObject = target;
        _lastTrackedObjectPosition = TrackedObject.transform.position;
        transform.position = TrackedObject.transform.position + Offset;
        transform.LookAt(TrackedObject.transform);
        transform.position = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        if (TrackedObject != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, TrackedObject.transform.position);
        }
    }

    public void SetLastTrackedPosition(Vector3 position)
    {
        _lastTrackedObjectPosition = position;
    }
}
