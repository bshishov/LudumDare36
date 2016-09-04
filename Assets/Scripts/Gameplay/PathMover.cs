using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathMover : NetworkBehaviour
{
    public enum PathMoverType
    {
        Once,
        PingPong,
        Loop,
        CloseLoop
    }

    [Serializable]
    public struct Waypoint
    {
        public Vector3 Position;
        public float WaitTime;
    }

    public float StartPointWaitTime;
    public Waypoint[] Waypoints;
    public float Speed = 1f;
    public PathMoverType MovementType;

    // STATE
#if UNITY_EDITOR
    [SerializeField]
    [ShowOnly]
#endif
    [SyncVar(hook = "OnStepSync")] private int _step = 0;

#if UNITY_EDITOR
    [SerializeField]
    [ShowOnly]
#endif
    //[SyncVar]
    private float _stepState = 0f;

    private Waypoint _initial;
    private Waypoint _last;
    private Waypoint _target;
    private float _departingTime;
    private IStatefullMovement<Waypoint> _movement;

    void Awake()
    {
        _initial = new Waypoint { Position = transform.position, WaitTime = StartPointWaitTime };}

    void Start()
    {
        _last = _initial;
        _target = Waypoints[0];
        _departingTime = Time.time + _last.WaitTime;

        var allWaypoints = new[] { _initial }.Concat(Waypoints).ToArray();

        switch (MovementType)
        {
            case PathMoverType.Loop:
            case PathMoverType.CloseLoop:
                _movement = new LoopMovement<Waypoint>(allWaypoints);
                break;
            case PathMoverType.PingPong:
                _movement = new PingPongMovement<Waypoint>(allWaypoints);
                break;
            case PathMoverType.Once:
                _movement = new OnceMovement<Waypoint>(allWaypoints);
                break;
        }
    }

    void Update()
    {
        if (Time.time > _departingTime)
        {
            _stepState += Time.deltaTime * (Speed / (_target.Position - _last.Position).magnitude);

            
            if (_stepState > 1f)
            {
                transform.position = Vector3.Lerp(_last.Position, _target.Position, 1f);

                if (isServer)
                {
                    _last = _target;
                    _target = _movement.GetNext();
                    _departingTime = Time.time + _last.WaitTime;
                    _stepState = 0;
                    _step = _movement.Step;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(_last.Position, _target.Position, _stepState);
            }
        }
    }

    void OnStepSync(int step)
    {
        if (!isServer)
        {
            Debug.LogFormat("{0} STEP SYNC = {1}", gameObject.name, step);
            _movement.SetStep(step);
            _step = step;
            _stepState = 0f;
            _last = _movement.Current;
            _target = _movement.Next;
            _departingTime = Time.time + _last.WaitTime;
        }
    }

    public Vector3 GetWaypointPoisition(int index)
    {
        return Waypoints[index].Position;
    }
    
    public Vector3 GetWaypointPoisition(Waypoint waypoint)
    {
        return waypoint.Position;
    }

    public void SetWaypointPoisition(int index, Vector3 position)
    {
        Waypoints[index].Position = position;
    }
    
    public void SetWaypointPoisition(Waypoint waypoint, Vector3 position)
    {
        waypoint.Position = position;
    }

    void OnDrawGizmosSelected()
    {

        //var style = new GUIStyle();
        //GUIStyle.none
        //style.normal.textColor = Color.red;
        var meshFilter = GetComponent<MeshFilter>();
        var lastPoint = transform.position;
        if (Application.isPlaying)
        {
            lastPoint = _initial.Position;
            Gizmos.DrawWireMesh(meshFilter.sharedMesh, lastPoint, transform.rotation, transform.localScale);
        }

        foreach (var waypoint in Waypoints)
        {
            var position = GetWaypointPoisition(waypoint);

            if (meshFilter != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, position, transform.rotation, transform.localScale);
            }

            //Vector3 pos = Handles.FreeMoveHandle(position, Quaternion.identity, .5f, new Vector3(.5f, .5f, .5f), Handles.RectangleCap);
            //Handles.PositionHandle(position, Quaternion.identity);
            Gizmos.DrawLine(lastPoint, position);
            lastPoint = position;
#if UNITY_EDITOR
            Handles.Label(position, "Wait: " + waypoint.WaitTime, GUIStyle.none);
#endif
        }

        if (MovementType == PathMoverType.CloseLoop)
        {
            if (Application.isPlaying)
                Gizmos.DrawLine(lastPoint, _initial.Position);
            else
                Gizmos.DrawLine(lastPoint, transform.position);
        }
    }

    public interface IStatefullMovement<out T>
    {
        T Current { get; }
        T Next { get; }
        T GetNext();
        int Step { get; }
        void Reset();
        void SetStep(int step);
    }

    public class LoopMovement<T> : IStatefullMovement<T>
    {
        public int Step { get { return _index; } }
        public T Current { get { return _data[_index]; } }
        public T Next { get { return _data[GetNextIndex()]; } }

        private readonly T[] _data;
        private int _index;

        public LoopMovement(params T[] data)
        {
            _data = data;
        }

        public T GetNext()
        {
            _index = GetNextIndex();
            return _data[_index];
        }

        int GetNextIndex()
        {
            if (_index + 1 >= _data.Length)
                return 0;
            return _index + 1;
        }

        public void Reset()
        {
            _index = 0;
        }

        public void SetStep(int step)
        {
            _index = step;
        }
    }

    public class OnceMovement<T> : IStatefullMovement<T>
    {
        public int Step { get { return _index; } }
        public T Current { get { return _data[_index]; } }
        public T Next{ get { return _data[GetNextIndex()]; } }

        private readonly T[] _data;
        private int _index;

        public OnceMovement(params T[] data)
        {
            _data = data;
        }

        public T GetNext()
        {
            return _data[GetNextIndex()];
        }

        int GetNextIndex()
        {
            if (_index + 1 >= _data.Length)
                return _index;
            return _index + 1;
        }

        public void Reset()
        {
            _index = 0;
        }

        public void SetStep(int step)
        {
            _index = step;
        }
    }

    public class PingPongMovement<T> : IStatefullMovement<T>
    {
        public T Current { get { return _data[_index]; } }
        public T Next { get { return _data[GetNextIndex()]; } }

        public int Step
        {
            get
            {
                // Forward
                if(_direction)
                    return _index;
                
                // backward
                return (_data.Length - 1) + _data.Length - _index - 1;
            }
        }

        private readonly T[] _data;
        private int _index;
        private bool _direction;

        public PingPongMovement(params T[] data)
        {
            _direction = true;
            _data = data;
        }

        public T GetNext()
        {
            if (_direction)
            {
                if (_index == _data.Length - 1)
                {
                    _index = _index - 1;
                    _direction = !_direction;
                }
                else
                {
                    _index++;
                }
            }
            else
            {
                if (_index == 0)
                {
                    _index = 1;
                    _direction = !_direction;
                }
                else
                {
                    _index--;
                }
            }
            
            return _data[_index];
        }

        int GetNextIndex()
        {
            if (_direction)
            {
                if (_index == _data.Length - 1)
                    return _index - 1;

                return _index + 1;
            }
            
            if (_index == 0)
                return 1;

            return _index - 1;
        }

        public void Reset()
        {
            _index = 0;
            _direction = true;
        }

        public void SetStep(int step)
        {
            if (step >= _data.Length)
            {
                _direction = false;
                _index = (_data.Length - 1) + _data.Length - step - 1;
            }
            else
                _index = step;
        }
    }
}