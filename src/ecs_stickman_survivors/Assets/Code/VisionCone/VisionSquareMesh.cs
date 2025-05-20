using UnityEngine;

namespace Code.VisionCone
{
    public class VisionSquareMesh : BaseVisionMesh
    {
        [Header("Square Settings")] [SerializeField]
        private float _width = 2f;

        [SerializeField] private float _height = 2f;
        [SerializeField] private int _segmentsX = 10;
        [SerializeField] private int _segmentsY = 10;

        private float _lastWidth;
        private float _lastHeight;
        private int _lastSegmentsX;
        private int _lastSegmentsY;

        protected override string MeshName => "VisionSquareMesh";

        protected override void GenerateMesh()
        {
            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();

            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;
            Quaternion inverseRotation = Quaternion.Inverse(rotation);

            for (int y = 0; y <= _segmentsY; y++)
            {
                float localY = -_height / 2f + _height * y / _segmentsY;

                for (int x = 0; x <= _segmentsX; x++)
                {
                    float localX = -_width / 2f + _width * x / _segmentsX;

                    Vector3 localPoint = new Vector3(localX, 0f, localY);
                    Vector3 worldPoint = origin + rotation * localPoint;
                    Vector3 dir = (worldPoint - origin).normalized;
                    float maxDist = localPoint.magnitude;

                    if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist, _obstacleMask))
                    {
                        localPoint = inverseRotation * (hit.point - origin);
                    }

                    _vertices.Add(localPoint);
                    _normals.Add(Vector3.up);
                    _uv.Add(new Vector2((float)x / _segmentsX, (float)y / _segmentsY));
                }
            }

            int vertexCountX = _segmentsX + 1;
            for (int y = 0; y < _segmentsY; y++)
            {
                for (int x = 0; x < _segmentsX; x++)
                {
                    int i = x + y * vertexCountX;

                    _triangles.Add(i);
                    _triangles.Add(i + vertexCountX);
                    _triangles.Add(i + 1);

                    _triangles.Add(i + 1);
                    _triangles.Add(i + vertexCountX);
                    _triangles.Add(i + vertexCountX + 1);
                }
            }

            Mesh mesh = _meshFilter.sharedMesh;
            mesh.Clear();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_triangles, 0);
            mesh.SetNormals(_normals);
            mesh.SetUVs(0, _uv);
        }


        protected override bool ParamsChanged()
        {
            return
                _lastWidth != _width ||
                _lastHeight != _height ||
                _lastSegmentsX != _segmentsX ||
                _lastSegmentsY != _segmentsY ||
                base.ParamsChanged();
        }

        protected override void CacheParams()
        {
            _lastWidth = _width;
            _lastHeight = _height;
            _lastSegmentsX = _segmentsX;
            _lastSegmentsY = _segmentsY;
            base.CacheParams();
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;

            for (int y = 0; y <= _segmentsY; y++)
            {
                float localY = -_height / 2f + _height * y / _segmentsY;

                for (int x = 0; x <= _segmentsX; x++)
                {
                    float localX = -_width / 2f + _width * x / _segmentsX;

                    Vector3 localPoint = new Vector3(localX, 0f, localY);
                    Vector3 worldPoint = origin + rotation * localPoint;
                    Vector3 dir = (worldPoint - origin).normalized;
                    float maxDist = localPoint.magnitude;

                    if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist, _obstacleMask))
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(origin, hit.point);
                        Gizmos.DrawSphere(hit.point, 0.025f);
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(origin, worldPoint);
                    }
                }
            }
#endif
        }
    }
}