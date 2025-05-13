using System.Collections.Generic;
using System.Linq;
using Plugins.ConcaveHullGenerator;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Code.Editors
{
    public class ColliderMeshEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Collider Mesh Generator Editor Window")]
        private static void OpenWindow()
        {
            GetWindow<ColliderMeshEditorWindow>().Show();
        }

        [BoxGroup("Collider Mesh Generation")] [LabelText("Target Mesh Filters")]
        public List<MeshFilter> targetMeshFilters = new();

        [BoxGroup("Collider Mesh Generation")] [LabelText("YOffset")]
        public float yOffset = 0.1f;

        [BoxGroup("Collider Mesh Generation")] [LabelText("Extrusion Thickness")]
        public float extrusion = 1f;

        [BoxGroup("Collider Mesh Generation")] [LabelText("Debug Material")]
        public Material debugMaterial;

        [BoxGroup("Collider Mesh Generation")] [LabelText("Concavity (-1 to 1)")]
        [Range(-1f, 1f)]
        public float concavity = 0.5f;

        [BoxGroup("Collider Mesh Generation")] [LabelText("Scale Factor")]
        [MinValue(0.01f)]
        public float scaleFactor = 1f;
        
        [BoxGroup("Collider Mesh Generation")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.9f, 0.4f)]
        private void GenerateCollider()
        {
            GameObject combinedGO = CreateCombinedMeshObject(targetMeshFilters, debugMaterial);
            if (combinedGO == null)
            {
                Debug.LogError("Failed to combine meshes.");
                return;
            }

            var targetMeshFilter = combinedGO.GetComponent<MeshFilter>();

            if (targetMeshFilter == null || targetMeshFilter.sharedMesh == null)
            {
                Debug.LogError("Missing target mesh filter.");
                return;
            }

            var worldPoints = targetMeshFilter.sharedMesh.vertices
                .Select(v => targetMeshFilter.transform.TransformPoint(v))
                .Select(p => new Vector3(p.x, 0, p.z))
                .ToList();

            var edgePoints = GenerateConcaveHullXZ(worldPoints, concavity, scaleFactor);
            var mesh = GenerateExtrudedMesh(edgePoints, yOffset, extrusion);

            GameObject container = new GameObject("Generated_Collider");
            container.transform.SetParent(targetMeshFilter.transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            var mf = container.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            var mr = container.AddComponent<MeshRenderer>();
            mr.sharedMaterial = debugMaterial;

            var col = container.AddComponent<MeshCollider>();
            col.sharedMesh = mesh;
            col.convex = false;

            Debug.Log("Collider mesh generated successfully.");
        }

        private List<Vector3> GenerateConcaveHullXZ(List<Vector3> points, double concavity = 0.5f, 
            double scaleFactor = 1.5f)
        {
            Hull.CleanUp();
            
            List<Node> nodes = new();
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                nodes.Add(new Node(p.x, p.z, i));
            }
            
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

        private GameObject CreateCombinedMeshObject(List<MeshFilter> meshFilters, Material mat = null)
        {
            var allVertices = new List<Vector3>();
            var allTriangles = new List<int>();
            var vertexMap = new Dictionary<Vector3, int>();

            float mergeThreshold = 0.001f;

            foreach (var mf in meshFilters)
            {
                if (mf == null || mf.sharedMesh == null) 
                    continue;

                var mesh = mf.sharedMesh;
                var matrix = mf.transform.localToWorldMatrix;
                var vertices = mesh.vertices;
                var triangles = mesh.triangles;

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var worldV = matrix.MultiplyPoint3x4(vertices[triangles[i + j]]);
                        worldV = new Vector3(worldV.x, 0, worldV.z);

                        int index;
                        if (!TryGetMergedIndex(worldV, allVertices, vertexMap, mergeThreshold, out index))
                        {
                            index = allVertices.Count;
                            allVertices.Add(worldV);
                            vertexMap[worldV] = index;
                        }

                        allTriangles.Add(index);
                    }
                }
            }

            var meshResult = new Mesh { name = "MergedMesh" };
            meshResult.SetVertices(allVertices);
            meshResult.SetTriangles(allTriangles, 0);
            meshResult.RecalculateNormals();

            var go = new GameObject("CombinedMeshObject");
            var mfNew = go.AddComponent<MeshFilter>();
            mfNew.sharedMesh = meshResult;

            var mrNew = go.AddComponent<MeshRenderer>();
            mrNew.sharedMaterial = mat ?? new Material(Shader.Find("Standard"));

            return go;
        }
        
        private bool TryGetMergedIndex(Vector3 point, List<Vector3> existing, Dictionary<Vector3, int> map,
            float threshold, out int index)
        {
            foreach (var kvp in map)
            {
                if (Vector3.Distance(kvp.Key, point) <= threshold)
                {
                    index = kvp.Value;
                    return true;
                }
            }

            index = -1;
            return false;
        }
    }
}