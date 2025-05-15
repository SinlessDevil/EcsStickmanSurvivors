using System.Collections.Generic;
using System.Linq;
using Plugins.ConcaveHull.Code;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Editors.СolliderMeshCreator
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

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("YOffset")]
        [SerializeField]
        private float _yOffset = 0.1f;

        [BoxGroup("Collider Mesh Generation")]
        [LabelText("Extrusion Thickness")]
        [SerializeField]
        private float _extrusion = 1f;

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
        [LabelText("Y Threshold Percent (0 = top only, 1 = full range)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _yThreshold = 0.05f;

        [BoxGroup("Collider Mesh Generation")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.9f, 0.4f)]
        private void GenerateCollider()
        {
            List<Vector3> worldPoints = MeshPointCollector.CollectWorldPoints(_targetMeshFilters);

            if (worldPoints.Count == 0)
            {
                Debug.LogError("No vertices found in the provided MeshFilters.");
                return;
            }

            List<Vector3> filteredPoints = YThresholdFilter.FilterTopPoints(worldPoints, _yThreshold);
            List<Node> nodes = filteredPoints.Select((p, i) => new Node(p.x, p.z, i)).ToList();

            Hull.CleanUp();
            Hull.SetConvexHull(nodes);
            List<Line> edges = Hull.SetConcaveHull(_concavity, _scaleFactor);

            List<Vector3> edgePoints = EdgeOutlineBuilder.BuildOutline(edges);
            Mesh mesh = GenerateExtrudedMesh(edgePoints, _yOffset, _extrusion);

            GameObject container = new GameObject("Generated_Collider");
            container.transform.SetParent(null);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            container.AddComponent<MeshFilter>().sharedMesh = mesh;
            container.AddComponent<MeshRenderer>().sharedMaterial = _debugMaterial;
            container.AddComponent<MeshCollider>().sharedMesh = mesh;
            container.GetComponent<MeshCollider>().convex = false;

            Debug.Log("Collider mesh generated successfully.");
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
                tris.AddRange(new[] { start + 2, start + 3, start + 1 });
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