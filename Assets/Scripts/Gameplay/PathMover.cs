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

  
    private int _step = 0;
    private float _stepState = 0f;

    private Waypoint _initial;
    private float _departingTime;
    private IStatefullMovement<Waypoint> _movement;

    void Awake()
    {
        _initial = new Waypoint { Position = transform.position, WaitTime = StartPointWaitTime };

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
        
        _departingTime = Time.time + _movement.Current.WaitTime;
    }

    void Start()
    {
    }

    void Update()
    {
        if (Time.time > _departingTime)
        {
            _stepState += Time.deltaTime * (Speed / (_movement.Current.Position - _movement.Next.Position).magnitude);
            
            if (_stepState > 1f)
            {
                transform.position = Vector3.Lerp(_movement.Current.Position, _movement.Next.Position, 1f);

                if (isServer)
                {
                    _movement.DoStep();
                    _departingTime = Time.time + _movement.Current.WaitTime;
                    _stepState = 0;
                    _step = _movement.StepIndex;
                    RpcStepChanged(_step);
                }
            }
            else
            {
                transform.position = Vector3.Lerp(_movement.Current.Position, _movement.Next.Position, _stepState);
            }
        }
    }

    [ClientRpc]
    void RpcStepChanged(int newStep)
    {
        if (!isServer)
        {
            _movement.SetStep(newStep);
            _step = newStep;
            _stepState = 0f;
            _departingTime = Time.time + _movement.Current.WaitTime - Network.GetAveragePing(Network.player) / 1000f;
        }
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
            if (meshFilter != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, waypoint.Position, transform.rotation, transform.localScale);
            }

            //Vector3 pos = Handles.FreeMoveHandle(position, Quaternion.identity, .5f, new Vector3(.5f, .5f, .5f), Handles.RectangleCap);
            //Handles.PositionHandle(position, Quaternion.identity);
            Gizmos.DrawLine(lastPoint, waypoint.Position);
            lastPoint = waypoint.Position;
#if UNITY_EDITOR
            Handles.Label(waypoint.Position, "Wait: " + waypoint.WaitTime, GUIStyle.none);
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
        T DoStep();
        int StepIndex { get; }
        void Reset();
        void SetStep(int step);
    }

    public class LoopMovement<T> : IStatefullMovement<T>
    {
        public int StepIndex { get { return _index; } }
        public T Current { get { return _data[_index]; } }
        public T Next { get { return _data[GetNextIndex()]; } }

        private readonly T[] _data;
        private int _index;

        public LoopMovement(params T[] data)
        {
            _data = data;
        }

        public T DoStep()
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
        public int StepIndex { get { return _index; } }
        public T Current { get { return _data[_index]; } }
        public T Next{ get { return _data[GetNextIndex()]; } }

        private readonly T[] _data;
        private int _index;

        public OnceMovement(params T[] data)
        {
            _data = data;
        }

        public T DoStep()
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

        public int StepIndex
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

        public T DoStep()
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
            {
                _direction = true;
                _index = step;
            }
        }
    }
}