using UnityEngine;
using System.Collections;

public class AnimatedRotate : MonoBehaviour
{
    public Vector3 RotationPoint;
    public Vector3 RotationAxis;
    public AnimationCurve Curve;
    public float Offset;

    [Range(0.1f, 20f)]
    public float Speed = 1f;

    [Range(0.1f, 360f)]
    public float Amount = 1f;

    private Quaternion _initialRotation;
    private Vector3 _initialPosition;
    private Vector3 _pivot;
    private Vector3 _rotationAxis;

    void Start ()
    {
        _initialRotation = transform.rotation;
        _initialPosition = transform.position;
        _pivot = transform.TransformPoint(RotationPoint);
        _rotationAxis = transform.TransformVector(RotationAxis).normalized;
    }
	
	void Update ()
	{
	    var rot = Quaternion.AngleAxis(Curve.Evaluate(Time.time * Speed + Offset) * Amount, _rotationAxis);
        var dir = _initialPosition - _pivot;
        transform.position = _initialPosition + rot * dir - dir;
        transform.rotation = rot * _initialRotation;
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(RotationPoint), 0.5f);
        
        Gizmos.DrawLine(transform.TransformPoint(RotationPoint), transform.TransformPoint(RotationPoint) + _rotationAxis);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_pivot, 0.5f);
    }
}
