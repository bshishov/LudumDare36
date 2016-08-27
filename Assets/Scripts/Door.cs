using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorState
    {
        Opened, 
        Opening,
        Closing,
        Closed,
        Waiting
    }

    public float OpeningSpeed = 1f;
    public float ClosingSpeed = 1f;
    public float OpenedY = -1f;
    public float TimeBeforeClose = 2f;

    public DoorState State { get; private set; }

    private float _currentState = 0f; // 0 = closed, 1 = opened
    private bool _isActivatingByAnyButton = false;
    private Vector3 _closedPosition;
    private Vector3 _openedPosition;
    private float _waitingTimer;

    void Start ()
    {
        State = DoorState.Closed;
        _closedPosition = transform.position;
        _openedPosition = transform.position + Vector3.up * OpenedY;
    }
	
	void Update ()
    {
	    if (State == DoorState.Opening)
	    {
	        if (_currentState < 1f)
	        {
	            _currentState += Time.deltaTime*OpeningSpeed;
	            transform.position = Vector3.Lerp(_closedPosition, _openedPosition, _currentState);
	        }
	        else
	        {
                SetState(DoorState.Opened);
	        }
	    }

	    if (State == DoorState.Closing)
	    {
	        if (_currentState > 0f)
	        {
	            _currentState -= Time.deltaTime*ClosingSpeed;
	            transform.position = Vector3.Lerp(_closedPosition, _openedPosition, _currentState);
	        }
            else
            {
                SetState(DoorState.Closed);
            }
        }

	    if (State == DoorState.Waiting)
	    {
	        _waitingTimer -= Time.deltaTime;
	        if (_waitingTimer < 0f)
	        {
	            SetState(DoorState.Closing);
	        }
	    }
    }

    // Called via SendMessage from Button
    void OnButtonActivate()
    {
        if(State != DoorState.Opened)
            SetState(DoorState.Opening);
    }

    // Called via SendMessage from Button
    void OnButtonDeactivate()
    {
        // if door is already closed
        if(State == DoorState.Closed)
            return;

        if (State == DoorState.Opening)
        {
            SetState(DoorState.Closing);
        }
        else
        {
            SetState(DoorState.Waiting);
            _waitingTimer = TimeBeforeClose;
        }
    }

    void SetState(DoorState state)
    {
        Debug.LogFormat("New state for {0}: {1}", gameObject.name, state);
        State = state;
    }
}
