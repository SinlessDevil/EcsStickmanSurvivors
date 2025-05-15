using System.Collections.Generic;
using System.Linq;
using Plugins.ConcaveHull.Code;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Editors
{
    public class ColliderMeshEditorWindow : OdinEditorWindow
    {
        private const float Epsilon = 0.001f;
        
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
        private float _yOffset = 0.1f;

        [FormerlySerializedAs("extrusion")]
        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Extrusion Thickness")]
        [SerializeField]
        private float _extrusion = 1f;

        [FormerlySerializedAs("debugMaterial")]
        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Debug Material")]
        [SerializeField]
        private Material _debugMaterial;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Concavity (-1 to 1)")] 
        [Range(-1f, 1f)] 
        [SerializeField]
        private float _concavity = 0.5f;

        [BoxGroup("Collider Mesh Generation")] 
        [LabelText("Scale Factor")] 
        [MinValue(0.01f)] 
        [SerializeField]
        private float _scaleFactor = 1f;

        [BoxGroup("Collider Mesh Generation")] 
        [LabelText("Y Threshold")] 
        [Range(0.001f, 15f)] 
        [SerializeField]
        private float _yThreshold = 0.05f;

        [BoxGroup("Collider Mesh Generation")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.9f, 0.4f)]
        private void GenerateCollider()
        {
            List<Vector3> worldPoints = new List<Vector3>();

            foreach (var targetMeshFilter in _targetMeshFilters)
            {
                if (targetMeshFilter == null || targetMeshFilter.sharedMesh == null)
                    continue;

                Mesh sharedMesh = targetMeshFilter.sharedMesh;
                Matrix4x4 matrix = targetMeshFilter.transform.localToWorldMatrix;

                worldPoints.AddRange(sharedMesh.vertices
                    .Select(v => matrix
                    .MultiplyPoint3x4(v)));
            }

            if (worldPoints.Count == 0)
            {
                Debug.LogError("No vertices found in the provided MeshFilters.");
                return;
            }

            float minY = worldPoints.Min(p => p.y);
            float maxY = worldPoints.Max(p => p.y);

            List<Vector3> filteredPoints = worldPoints
                .Where(p => Mathf.Abs(p.y - maxY) <= _yThreshold || Mathf.Abs(p.y - minY) <= _yThreshold)
                .Select(p => new Vector3(p.x, 0, p.z))
                .ToList();

            List<Vector3> edgePoints = GenerateConcaveHullXZ(filteredPoints, _concavity, _scaleFactor);
            Mesh mesh = GenerateExtrudedMesh(edgePoints, _yOffset, _extrusion);

            GameObject container = new GameObject("Generated_Collider");
            container.transform.SetParent(null);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            MeshFilter meshFilter = container.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshRenderer = container.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _debugMaterial;

            MeshCollider meshCollider = container.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = false;

            Debug.Log("Collider mesh generated successfully.");
        }

        private List<Vector3> GenerateConcaveHullXZ(List<Vector3> points, double concavity, double scaleFactor)
        {
            Hull.CleanUp();
            List<Node> nodes = points
                .Select((p, i) => new Node(p.x, p.z, i))
                .ToList();
            Hull.SetConvexHull(nodes);
            List<Line> edges = Hull.SetConcaveHull(concavity, scaleFactor);
            return BuildOrderedOutlineFromEdges(edges);
        }

        private List<Vector3> BuildOrderedOutlineFromEdges(List<Line> unorderedEdges)
        {
            List<Vector3> outline = new List<Vector3>();

            if (unorderedEdges == null || unorderedEdges.Count == 0)
                return outline;
            
            List<Line> edges = new List<Line>(unorderedEdges);
            
            Line current = edges[0];
            edges.RemoveAt(0);

            Vector2 start = new Vector2((float)current.Nodes[0].X, (float)current.Nodes[0].Y);
            Vector2 end = new Vector2((float)current.Nodes[1].X, (float)current.Nodes[1].Y);

            outline.Add(new Vector3(start.x, 0, start.y));
            outline.Add(new Vector3(end.x, 0, end.y));

            while (edges.Count > 0)
            {
                Vector2 last = new Vector2(outline[^1].x, outline[^1].z);
                
                int index = edges.FindIndex(e =>
                    Vector2.Distance(new Vector2((float)e.Nodes[0].X, (float)e.Nodes[0].Y), last) < Epsilon ||
                    Vector2.Distance(new Vector2((float)e.Nodes[1].X, (float)e.Nodes[1].Y), last) < Epsilon);

                if (index == -1)
                    break;

                Line edge = edges[index];
                edges.RemoveAt(index);

                Node nextNode = Vector2.Distance(new Vector2(
                    (float)edge.Nodes[0].X, 
                    (float)edge.Nodes[0].Y), last) < Epsilon
                    ? edge.Nodes[1]
                    : edge.Nodes[0];

                outline.Add(new Vector3((float)nextNode.X, 0, (float)nextNode.Y));
            }

            return outline;
        }
        
        private Mesh GenerateExtrudedMesh(List<Vector3> path, float height, float thickness)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

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

                tris.AddRange(new[] { start + 2, start + 1, start + 0 });
                tris.AddRange(new[] { start + 2, start + 3, start + 1 });;
            }

            Mesh mesh = new Mesh();
            mesh.name = "ColliderMesh";
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}