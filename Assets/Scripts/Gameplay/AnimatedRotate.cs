using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


[RequireComponent(typeof(PivotRotation))]
public class AnimatedRotate : MonoBehaviour
{
    public AnimationCurve Curve;
    public float Offset;

    [Range(0.1f, 20f)]
    public float Speed = 1f;

    [Range(0.1f, 360f)]
    public float Amount = 1f;

    private PivotRotation _pivotRotation;

    void Start ()
    {
        _pivotRotation = GetComponent<PivotRotation>();
    }
	
	void Update ()
	{
        _pivotRotation.Value = Curve.Evaluate(Time.time * Speed + Offset) * Amount;
	}
}
