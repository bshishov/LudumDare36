using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class PivotRotation : MonoBehaviour
    {
        public float Value;
        public Vector3 LocalPivot;
        public Vector3 LocalRotationAxis;

        private Quaternion _initialRotation;
        private Vector3 _initialPosition;
        private Vector3 _worldPivot;
        private Vector3 _worldRotationAxis;
    
        void Awake()
        {
            _initialRotation = transform.rotation;
            _initialPosition = transform.position;
            _worldPivot = transform.TransformPoint(LocalPivot);
            _worldRotationAxis = transform.TransformVector(LocalRotationAxis).normalized;
        }
	
        void Update()
        {
            var rot = Quaternion.AngleAxis(Value, _worldRotationAxis);
            var dir = _initialPosition - _worldPivot;
            transform.position = _initialPosition + rot * dir - dir;
            transform.rotation = rot * _initialRotation;
        }

        void OnDrawGizmosSelected()
        {
            var wrAxis = transform.TransformVector(LocalRotationAxis).normalized;
            var wrPivot = transform.TransformPoint(LocalPivot);

            Gizmos.DrawWireSphere(wrPivot, 0.4f);
            Gizmos.DrawLine(wrPivot, transform.TransformPoint(LocalPivot) + wrAxis);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_worldPivot, 0.3f);
            Gizmos.DrawLine(_worldPivot, _worldPivot + _worldRotationAxis);

        
            if (!Application.isPlaying && Mathf.Abs(Value) > Mathf.Epsilon)
            {
                var meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Gizmos.color = Color.white;
                    var rot = Quaternion.AngleAxis(Value, wrAxis);
                    var dir = transform.position - wrPivot;
                    Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position + rot * dir - dir, rot * transform.rotation,
                        transform.localScale);
                }
            }
        }
    }
}
