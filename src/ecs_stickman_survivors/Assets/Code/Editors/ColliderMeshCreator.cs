using System.Collections.Generic;
using System.Linq;
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

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Target Mesh Filters")]
        public List<MeshFilter> targetMeshFilters = new();

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("YOffset")]
        public float yOffset = 0.1f;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Extrusion Thickness")]
        public float extrusion = 1f;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Debug Material")]
        public Material debugMaterial;

        [BoxGroup("Collider Mesh Generation")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.9f, 0.4f)]
        private void GenerateCollider()
        {
            if (targetMeshFilters == null || targetMeshFilters.Count == 0)
            {
                Debug.LogError("No target mesh filters assigned.");
                return;
            }

            // Создаём объединённый объект и получаем mesh
            GameObject combinedGO = CreateCombinedMeshObject(targetMeshFilters, debugMaterial);
            if (combinedGO == null)
            {
                Debug.LogError("Failed to combine meshes.");
                return;
            }

            // Создаём временный MeshFilter чтобы извлечь outline
            var mfTemp = combinedGO.GetComponent<MeshFilter>();
            var allPoints = ExtractMeshOutlineXZ(mfTemp);

            if (allPoints.Count == 0)
            {
                Debug.LogError("No valid mesh data found.");
                return;
            }

            var orderedPoints = ConvexHullXZ(allPoints);
            var mesh = GenerateExtrudedMesh(orderedPoints, yOffset, extrusion);

            GameObject container = new GameObject("Generated_Collider");
            container.transform.SetParent(null);
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

        private GameObject CreateCombinedMeshObject(List<MeshFilter> meshFilters, Material mat = null)
        {
            var combine = new List<CombineInstance>();

            foreach (var аа in meshFilters)
            {
                if (аа == null || аа.sharedMesh == null) continue;

                combine.Add(new CombineInstance
                {
                    mesh = аа.sharedMesh,
                    transform = аа.transform.localToWorldMatrix
                });
            }

            if (combine.Count == 0) return null;

            var combinedMesh = new Mesh { name = "CombinedMesh" };
            combinedMesh.CombineMeshes(combine.ToArray(), true, true, false);

            var go = new GameObject("CombinedMeshObject");
            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = combinedMesh;

            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat ?? new Material(Shader.Find("Standard"));

            return go;
        }

        private List<Vector3> ExtractMeshOutlineXZ(MeshFilter meshFilter)
        {
            var mesh = meshFilter.sharedMesh;
            var vertices = mesh.vertices.Select(v => meshFilter.transform.TransformPoint(v)).ToArray();
            var triangles = mesh.triangles;

            var edgeCount = new Dictionary<(Vector3, Vector3), int>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3[] triVerts =
                {
                    new(vertices[triangles[i]].x, 0, vertices[triangles[i]].z),
                    new(vertices[triangles[i + 1]].x, 0, vertices[triangles[i + 1]].z),
                    new(vertices[triangles[i + 2]].x, 0, vertices[triangles[i + 2]].z)
                };

                void AddEdge(Vector3 a, Vector3 b)
                {
                    var key = (a, b);
                    var reverseKey = (b, a);
                    if (edgeCount.ContainsKey(reverseKey)) edgeCount[reverseKey]++;
                    else if (edgeCount.ContainsKey(key)) edgeCount[key]++;
                    else edgeCount[key] = 1;
                }

                AddEdge(triVerts[0], triVerts[1]);
                AddEdge(triVerts[1], triVerts[2]);
                AddEdge(triVerts[2], triVerts[0]);
            }

            var boundaryEdges = edgeCount.Where(e => e.Value == 1).Select(e => e.Key).ToList();

            if (boundaryEdges.Count == 0)
            {
                Debug.LogWarning($"No boundary edges found for mesh: {meshFilter.name}");
                return new List<Vector3>();
            }

            var outline = new List<Vector3> { boundaryEdges[0].Item1, boundaryEdges[0].Item2 };
            boundaryEdges.RemoveAt(0);

            int safety = 10000;
            while (boundaryEdges.Count > 0 && safety-- > 0)
            {
                var last = outline[^1];
                var nextEdge = boundaryEdges.Find(e => e.Item1 == last || e.Item2 == last);

                if (nextEdge.Equals(default((Vector3, Vector3))))
                {
                    Debug.LogWarning("Cannot find next connected edge, outline may be broken.");
                    break;
                }

                outline.Add(nextEdge.Item1 == last ? nextEdge.Item2 : nextEdge.Item1);
                boundaryEdges.Remove(nextEdge);
            }

            if (safety <= 0)
                Debug.LogError("Infinite loop protection triggered in outline generation.");

            return outline;
        }

        private List<Vector3> ConvexHullXZ(List<Vector3> points)
        {
            var sorted = points.Distinct().OrderBy(p => p.x).ThenBy(p => p.z).ToList();

            List<Vector3> hull = new();
            foreach (var p in sorted)
            {
                while (hull.Count >= 2 && CrossXZ(hull[^2], hull[^1], p) <= 0)
                    hull.RemoveAt(hull.Count - 1);
                hull.Add(p);
            }

            int t = hull.Count + 1;
            for (int i = sorted.Count - 2; i >= 0; i--)
            {
                var p = sorted[i];
                while (hull.Count >= t && CrossXZ(hull[^2], hull[^1], p) <= 0)
                    hull.RemoveAt(hull.Count - 1);
                hull.Add(p);
            }

            hull.RemoveAt(hull.Count - 1);
            return hull;
        }

        private float CrossXZ(Vector3 a, Vector3 b, Vector3 c)
        {
            return (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x);
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

                tris.AddRange(new[] { start + 0, start + 2, start + 1 });
                tris.AddRange(new[] { start + 1, start + 2, start + 3 });
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