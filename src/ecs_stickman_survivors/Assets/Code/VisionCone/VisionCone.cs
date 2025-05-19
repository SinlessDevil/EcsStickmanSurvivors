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

                UpdateMainLevel(_meshFilter, _visionRange);
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
            Vector3 origin = transform.position;
            int minmax = Mathf.RoundToInt(_visionAngle / 2f);
            float step = Mathf.Clamp(_visionAngle / _precision, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step)
            {
                float angle = (i + 90f) * Mathf.Deg2Rad;
                Vector3 dirLocal = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 dirWorld = transform.TransformDirection(dirLocal);

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

            int minmax = Mathf.RoundToInt(_visionAngle / 2f);
            float step = Mathf.Clamp(_visionAngle / _precision, 0.01f, minmax);
            int index = 1;

            Vector3 worldOrigin = transform.position;
            Quaternion worldRotation = transform.rotation;
            Quaternion inverseRotation = Quaternion.Inverse(worldRotation);

            for (float i = -minmax; i <= minmax; i += step)
            {
                float angle = (i + 90f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * range;

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
    }
}
