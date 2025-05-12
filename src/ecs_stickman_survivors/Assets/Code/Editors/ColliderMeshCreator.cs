using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class ColliderMeshCreator : MonoBehaviour
{
    [Header("Source Mesh")] public MeshFilter islandMeshFilter;
    [Header("Generated Collider Settings")] public float yOffset = 0.1f;
    public float extrusion = 1f;
    public Material debugMaterial;

    private List<Vector3> _edgePoints;

    [Button]
    private void GenerateColliderMesh()
    {
        if (islandMeshFilter == null || islandMeshFilter.sharedMesh == null)
        {
            Debug.LogError("Missing island mesh");
            return;
        }

        // Step 1: Get mesh outline
        _edgePoints = ExtractMeshOutlineXZ(islandMeshFilter);

        // Step 2: Create mesh from outline
        var mesh = GenerateExtrudedMesh(_edgePoints, yOffset, extrusion);

        // Step 3: Create GameObject with MeshCollider
        var obj = new GameObject("Generated_Collider");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        var mf = obj.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        var mr = obj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = debugMaterial;

        var collider = obj.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.convex = false;
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
            var last = outline[outline.Count - 1];
            var nextEdge = boundaryEdges.Find(e => e.Item1 == last || e.Item2 == last);
            if (nextEdge.Item1 == last)
                outline.Add(nextEdge.Item2);
            else
                outline.Add(nextEdge.Item1);

            boundaryEdges.Remove(nextEdge);
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

    private void OnDrawGizmosSelected()
    {
        if (_edgePoints == null || _edgePoints.Count == 0)
            return;

        Gizmos.color = Color.magenta;
        foreach (var point in _edgePoints)
        {
            Gizmos.DrawSphere(point, 0.3f);
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < _edgePoints.Count; i++)
        {
            Vector3 current = _edgePoints[i];
            Vector3 next = _edgePoints[(i + 1) % _edgePoints.Count];
            Gizmos.DrawLine(current, next);
        }
    }
}
