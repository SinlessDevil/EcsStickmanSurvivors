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

        [BoxGroup("Collider Mesh Generation")] [LabelText("Target Mesh Filters")]
        public List<MeshFilter> targetMeshFilters = new();

        [BoxGroup("Collider Mesh Generation")] [LabelText("YOffset")]
        public float yOffset = 0.1f;

        [BoxGroup("Collider Mesh Generation")] [LabelText("Extrusion Thickness")]
        public float extrusion = 1f;

        [BoxGroup("Collider Mesh Generation")] [LabelText("Debug Material")]
        public Material debugMaterial;

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

            var edgePoints = ExtractMeshOutlineXZ(targetMeshFilter);
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
                    new Vector3(vertices[triangles[i]].x, 0, vertices[triangles[i]].z),
                    new Vector3(vertices[triangles[i + 1]].x, 0, vertices[triangles[i + 1]].z),
                    new Vector3(vertices[triangles[i + 2]].x, 0, vertices[triangles[i + 2]].z)
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
            var outline = new List<Vector3> { boundaryEdges[0].Item1, boundaryEdges[0].Item2 };
            boundaryEdges.RemoveAt(0);

            while (boundaryEdges.Count > 0)
            {
                var last = outline[^1];
                int index = boundaryEdges.FindIndex(e => e.Item1 == last || e.Item2 == last);

                if (index == -1)
                {
                    Debug.LogWarning("Unclosed loop detected. Ending outline early.");
                    break;
                }

                var nextEdge = boundaryEdges[index];
                outline.Add(nextEdge.Item1 == last ? nextEdge.Item2 : nextEdge.Item1);
                boundaryEdges.RemoveAt(index);
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
                if (mf == null || mf.sharedMesh == null) continue;

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