using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject TrackedObject;
    public float SmoothTime = 0.5f;
    public float LookAhead = 1.2f;
    public Vector3 Offset = new Vector3(0f, 4f, -1f);

    private Vector3 _velocity;
    private Rigidbody _rigidbody;

    void Start ()
    {
	}
	
	void Update ()
	{
        if (!TrackedObject)
            return;

	    if (_rigidbody != null)
	    {
	        transform.position = Vector3.SmoothDamp(transform.position,
	            TrackedObject.transform.position + Offset + _rigidbody.velocity * LookAhead,
	            ref _velocity,
	            SmoothTime);
	    }
	    else
	    {
            transform.position = Vector3.SmoothDamp(transform.position,
                TrackedObject.transform.position + Offset,
                ref _velocity,
                SmoothTime);
        }
	}

    public void SetTarget(GameObject target)
    {
        TrackedObject = target;
        _rigidbody = target.GetComponent<Rigidbody>();
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
}
