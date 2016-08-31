using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActivatorProxy))]
public class ActivatableTransform : MonoBehaviour
{
    public Vector3 DeltaPosition = Vector3.zero;
    public float Speed = 2f;

    private float _state = 0f;
    private ActivatorProxy _activator;
    private Vector3 _initialPosition;
    private Vector3 _targedPosition;

    void Awake()
    {
        _initialPosition = transform.position;
        _targedPosition = _initialPosition + DeltaPosition;
    }

    void Start ()
    {
        _activator = GetComponent<ActivatorProxy>();
    }
	
	void Update ()
    {
	    if (_activator.IsActivated && _state < 1f)
	    {
	        _state += Time.deltaTime * Speed;
            transform.position = Vector3.Lerp(_initialPosition, _targedPosition, _state);
        }

        if (!_activator.IsActivated && _state > 0f)
        {
            _state -= Time.deltaTime * Speed;
            transform.position = Vector3.Lerp(_initialPosition, _targedPosition, _state);
        }
	}
}
