using System;
using UnityEngine;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathMover : MonoBehaviour
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

    private float _state = 0f;
    private Waypoint _initial;
    private Waypoint _last;
    private Waypoint _target;
    private float _distance;
    private float _departingTime;
    private IMovement<Waypoint> _movement;

    void Start()
    {
        _initial = new Waypoint {Position = transform.position, WaitTime = StartPointWaitTime };
        _last = _initial;
        _target = Waypoints[0];
        _distance = (_target.Position - _last.Position).magnitude;
        _departingTime = Time.time + _last.WaitTime;

        var allWaypoints = new[] {_initial}.Concat(Waypoints).ToArray();

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

    void Update ()
	{
	    if (Time.time > _departingTime)
	    {
	        _state += Time.deltaTime * (Speed / _distance);

	        if (_state > 1f)
	        {
                transform.position = Vector3.Lerp(_last.Position, _target.Position, 1f);

                _last = _target;
                _target = _movement.GetNext();
                _departingTime = Time.time + _last.WaitTime;
                _distance = (_target.Position - _last.Position).magnitude;
                _state = 0;
            }
	        else
	        {
	            transform.position = Vector3.Lerp(_last.Position, _target.Position, _state);
	        }
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

    public interface IMovement<out T>
    {
        T GetNext();
        void Reset();
    }

    public class LoopMovement<T> : IMovement<T>
    {
        private readonly T[] _data;
        private int _index;

        public LoopMovement(params T[] data)
        {
            _data = data;
        }

        public T GetNext()
        {
            _index++;
            if (_index >= _data.Length)
                _index = 0;

            return _data[_index];
        }

        public void Reset()
        {
            _index = 0;
        }
    }

    public class OnceMovement<T> : IMovement<T>
    {
        private readonly T[] _data;
        private int _index;

        public OnceMovement(params T[] data)
        {
            _data = data;
        }

        public T GetNext()
        {
            if(_index < _data.Length - 1)
                _index++;
            return _data[_index];
        }

        public void Reset()
        {
            _index = 0;
        }
    }

    public class PingPongMovement<T> : IMovement<T>
    {
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

        public void Reset()
        {
            _index = 0;
            _direction = true;
        }
    }
}
