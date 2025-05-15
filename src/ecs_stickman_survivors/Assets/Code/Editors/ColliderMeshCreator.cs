using System.Collections.Generic;
using System.Linq;
using Plugins.ConcaveHullGenerator;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Editors
{
    public class ColliderMeshEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Collider Mesh Generator Editor Window")]
        private static void OpenWindow()
        {
            GetWindow<ColliderMeshEditorWindow>().Show();
        }

        [FormerlySerializedAs("targetMeshFilters")]
        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Target Mesh Filters")]
        [SerializeField]
        private List<MeshFilter> _targetMeshFilters = new();

        [FormerlySerializedAs("yOffset")]
        [BoxGroup("Collider Mesh Generation")]
        [LabelText("YOffset")]
        [SerializeField]
        private float _yOffset = 10f;

        [FormerlySerializedAs("extrusion")]
        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Extrusion Thickness")]
        [SerializeField]
        private float _extrusion = 50f;

        [FormerlySerializedAs("debugMaterial")]
        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Debug Material")]
        [SerializeField]
        private Material _debugMaterial;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Concavity (-1 to 1)")]
        [Range(-1f, 1f)]
        [SerializeField]
        private float _concavity = 0f;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Scale Factor")]
        [MinValue(0.01f)]
        [SerializeField]
        private float _scaleFactor = 1f;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Y Threshold Percent (0 = top only, 1 = full range)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _yThreshold = 0.7f;

        [BoxGroup("Collider Mesh Generation")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.9f, 0.4f)]
        private void GenerateCollider()
        {
            var worldPoints = new List<Vector3>();

            foreach (var targetMeshFilter in _targetMeshFilters)
            {
                if (targetMeshFilter == null || targetMeshFilter.sharedMesh == null)
                    continue;

                var sharedMesh = targetMeshFilter.sharedMesh;
                var matrix = targetMeshFilter.transform.localToWorldMatrix;

                worldPoints.AddRange(sharedMesh.vertices
                    .Select(v => matrix.MultiplyPoint3x4(v)));
            }

            if (worldPoints.Count == 0)
            {
                Debug.LogError("No vertices found in the provided MeshFilters.");
                return;
            }

            float minY = worldPoints.Min(p => p.y);
            float maxY = worldPoints.Max(p => p.y);
            float rangeY = maxY - minY;
            float cutoffY = maxY - rangeY * _yThreshold;

            var filteredPoints = worldPoints
                .Where(p => p.y >= cutoffY)
                .Select(p => new Vector3(p.x, 0, p.z))
                .ToList();

            var edgePoints = GenerateConcaveHullXZ(filteredPoints, _concavity, _scaleFactor);
            var mesh = GenerateExtrudedMesh(edgePoints, _yOffset, _extrusion);

            GameObject container = new GameObject("Generated_Collider");
            container.transform.SetParent(null);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            var meshFilter = container.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            var meshRenderer = container.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _debugMaterial;

            var meshCollider = container.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = false;

            Debug.Log("Collider mesh generated successfully.");
        }

        private List<Vector3> GenerateConcaveHullXZ(List<Vector3> points, double concavity = 0.5,
            double scaleFactor = 1.5f)
        {
            Hull.CleanUp();

            List<Node> nodes = points.Select((p, i) => new Node(p.x, p.z, i)).ToList();

            Hull.SetConvexHull(nodes);
            Hull.SetConcaveHull(concavity, scaleFactor);

            var outline = new List<Vector3>();
            var edges = Hull.HullConcaveEdges;

            if (edges.Count == 0)
                return outline;

            var current = edges[0];
            outline.Add(new Vector3((float)current.Nodes[0].X, 0, (float)current.Nodes[0].Y));
            outline.Add(new Vector3((float)current.Nodes[1].X, 0, (float)current.Nodes[1].Y));

            edges.RemoveAt(0);

            while (edges.Count > 0)
            {
                var last = outline[^1];
                int index = edges.FindIndex(e =>
                    (float)e.Nodes[0].X == last.x && (float)e.Nodes[0].Y == last.z ||
                    (float)e.Nodes[1].X == last.x && (float)e.Nodes[1].Y == last.z);

                if (index == -1)
                    break;

                var e = edges[index];
                var next = ((float)e.Nodes[0].X == last.x && (float)e.Nodes[0].Y == last.z)
                    ? e.Nodes[1]
                    : e.Nodes[0];

                outline.Add(new Vector3((float)next.X, 0, (float)next.Y));
                edges.RemoveAt(index);
            }

            return outline;
        }

        private Mesh GenerateExtrudedMesh(List<Vector3> path, float height, float thickness)
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();

            for (int i = 0; i < path.Count; i++)
            {
                Vector3 baseA = path[i] + Vector3.up * height;
                Vector3 baseB = path[(i + 1) % path.Count] + Vector3.up * height;
                Vector3 lowerA = baseA - Vector3.up * thickness;
                Vector3 lowerB = baseB - Vector3.up * thickness;

                int start = verts.Count;
                verts.Add(baseA);
                verts.Add(baseB);
                verts.Add(lowerA);
                verts.Add(lowerB);

                tris.AddRange(new[] { start + 0, start + 1, start + 2 });
                tris.AddRange(new[] { start + 1, start + 3, start + 2 });
            }

            var mesh = new Mesh();
            mesh.name = "ColliderMesh";
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
