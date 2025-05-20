using System.Collections.Generic;
using UnityEngine;

namespace Code.VisionCone
{
    public class VisionCircleMesh : BaseVisionMesh
    {
        private Vector3[] _precomputedDirs;

        protected override string MeshName => "VisionCircleMesh";

        protected override void GenerateMesh()
        {
            if (_precomputedDirs == null || _precomputedDirs.Length == 0)
                RecalculateDirections();

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
                Vector3 dir = dirLocal * _visionRange;
                Vector3 dirWorld = worldRotation * dir.normalized;

                if (Physics.Raycast(worldOrigin, dirWorld, out RaycastHit hit, _visionRange, _obstacleMask.value))
                    dir = dir.normalized * hit.distance;

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

            Mesh m = _meshFilter.sharedMesh;
            m.Clear();
            m.SetVertices(_vertices);
            m.SetTriangles(_triangles, 0);
            m.SetNormals(_normals);
            m.SetUVs(0, _uv);
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
