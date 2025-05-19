using System.Collections.Generic;
using UnityEngine;

namespace Code.VisionCone
{
    [ExecuteAlways]
    public class VisionCone : MonoBehaviour
    {
        [Header("Vision")]
        [SerializeField] private float _visionAngle = 360f;
        [SerializeField] private float _visionRange = 1f;
        [SerializeField] private LayerMask _obstacleMask = ~0;
        [Header("Material")]
        [SerializeField] private Material _coneMaterial;
        [SerializeField] private int _sortOrder = 1;
        [Header("Optimization")]
        [SerializeField] private int _precision = 300;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private bool _isInitialized;
        
        private float _lastAngle;
        private float _lastRange;
        private int _lastPrecision;
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        
        private Vector3[] _precomputedDirs;
        
        private readonly List<Vector3> _vertices = new();
        private readonly List<int> _triangles = new();
        private readonly List<Vector3> _normals = new();
        private readonly List<Vector2> _uv = new();
        
        private void OnEnable()
        {
            if (_isInitialized) 
                return;

            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null)
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
                _meshFilter = gameObject.AddComponent<MeshFilter>();

            if (_coneMaterial != null)
                _meshRenderer.sharedMaterial = _coneMaterial;

            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            _meshRenderer.allowOcclusionWhenDynamic = false;
            _meshRenderer.sortingOrder = _sortOrder;

            if (_meshFilter.sharedMesh == null)
                _meshFilter.sharedMesh = new Mesh { name = "VisionCone" };

            _isInitialized = true;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (!_isInitialized)
                    OnEnable();

                if (ParamsChanged())
                {
                    UpdateMainLevel(_meshFilter, _visionRange);
                    CacheParams();
                }
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && _meshFilter != null)
            {
                UpdateMainLevel(_meshFilter, _visionRange);
            }
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;

            int minmax = Mathf.RoundToInt(_visionAngle / 2f);
            float step = Mathf.Clamp(_visionAngle / _precision, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step)
            {
                float angleRad = (i + 90f) * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angleRad);
                float sin = Mathf.Sin(angleRad);

                Vector3 dirLocal = new Vector3(cos, 0f, sin);
                Vector3 dirWorld = rotation * dirLocal;

                if (Physics.Raycast(origin, dirWorld, out RaycastHit hit, _visionRange, _obstacleMask.value))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, origin + dirWorld * hit.distance);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, origin + dirWorld * _visionRange);
                }
            }
#endif
        }

        private void UpdateMainLevel(MeshFilter mesh, float range)
        {
            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();

            _vertices.Add(Vector3.zero);
            _normals.Add(Vector3.up);
            _uv.Add(Vector2.zero);

            Vector3 worldOrigin = transform.position;
            Quaternion worldRotation = transform.rotation;
            Quaternion inverseRotation = Quaternion.Inverse(worldRotation);

            int index = 1;

            foreach (var dirLocal in _precomputedDirs)
            {
                Vector3 dir = dirLocal * range;
                Vector3 dirWorld = worldRotation * dir.normalized;

                if (Physics.Raycast(worldOrigin, dirWorld, out RaycastHit hit, range, _obstacleMask.value))
                {
                    dir = dir.normalized * hit.distance;
                }

                Vector3 localDir = inverseRotation * dir;
                _vertices.Add(localDir);
                _normals.Add(Vector3.up);
                _uv.Add(Vector2.zero);

                if (index > 1)
                {
                    _triangles.Add(0);
                    _triangles.Add(index - 1);
                    _triangles.Add(index);
                }

                index++;
            }

            Mesh m = mesh.sharedMesh;
            m.Clear();
            m.SetVertices(_vertices);
            m.SetTriangles(_triangles, 0);
            m.SetNormals(_normals);
            m.SetUVs(0, _uv);
        }
        
        private bool ParamsChanged() =>
            !Mathf.Approximately(_lastAngle, _visionAngle) ||
            !Mathf.Approximately(_lastRange, _visionRange) ||
            _lastPrecision != _precision ||
            _lastPosition != transform.position ||
            _lastRotation != transform.rotation;

        private void CacheParams()
        {
            _lastAngle = _visionAngle;
            _lastRange = _visionRange;
            _lastPrecision = _precision;
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;

            RecalculateDirections();
        }
        
        private void RecalculateDirections()
        {
            int minmax = Mathf.RoundToInt(_visionAngle / 2f);
            float step = Mathf.Clamp(_visionAngle / _precision, 0.01f, minmax);

            List<Vector3> dirs = new();
            for (float i = -minmax; i <= minmax; i += step)
            {
                float angle = (i + 90f) * Mathf.Deg2Rad;
                dirs.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
            }

            _precomputedDirs = dirs.ToArray();
        }
    }
}
