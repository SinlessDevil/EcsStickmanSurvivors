using System.Collections.Generic;
using UnityEngine;

namespace Code.VisionCone
{
    [ExecuteAlways]
    public class VisionCone : MonoBehaviour
    {
        [Header("Vision")]
        public float vision_angle = 30f;
        public float vision_range = 5f;
        public LayerMask obstacle_mask = ~0;

        [Header("Material")]
        public Material cone_material;
        public int sort_order = 1;

        [Header("Optimization")]
        public int precision = 60;

        private MeshRenderer render;
        private MeshFilter mesh;

        private bool isInitialized;

        private void OnEnable()
        {
            if (isInitialized) 
                return;

            render = GetComponent<MeshRenderer>();
            if (render == null)
                render = gameObject.AddComponent<MeshRenderer>();

            mesh = GetComponent<MeshFilter>();
            if (mesh == null)
                mesh = gameObject.AddComponent<MeshFilter>();

            if (cone_material != null)
                render.sharedMaterial = cone_material;

            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            render.allowOcclusionWhenDynamic = false;
            render.sortingOrder = sort_order;

            if (mesh.sharedMesh == null)
                mesh.sharedMesh = new Mesh { name = "VisionCone" };

            isInitialized = true;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (!isInitialized)
                    OnEnable();

                UpdateMainLevel(mesh, vision_range);
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && mesh != null)
            {
                UpdateMainLevel(mesh, vision_range);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position;
            int minmax = Mathf.RoundToInt(vision_angle / 2f);
            float step = Mathf.Clamp(vision_angle / precision, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step)
            {
                float angle = (i + 90f) * Mathf.Deg2Rad;
                Vector3 dirLocal = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 dirWorld = transform.TransformDirection(dirLocal);

                if (Physics.Raycast(origin, dirWorld, out RaycastHit hit, vision_range, obstacle_mask.value))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, origin + dirWorld * hit.distance);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, origin + dirWorld * vision_range);
                }
            }
        }

        private void UpdateMainLevel(MeshFilter mesh, float range)
        {
            List<Vector3> vertices = new List<Vector3> { Vector3.zero };
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3> { Vector3.up };
            List<Vector2> uv = new List<Vector2> { Vector2.zero };

            int minmax = Mathf.RoundToInt(vision_angle / 2f);
            float step = Mathf.Clamp(vision_angle / precision, 0.01f, minmax);
            int index = 1;

            for (float i = -minmax; i <= minmax; i += step)
            {
                float angle = (i + 90f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * range;

                Vector3 worldOrigin = transform.position;
                Vector3 dirWorld = transform.TransformDirection(dir.normalized);
                if (Physics.Raycast(worldOrigin, dirWorld, out RaycastHit hit, range, obstacle_mask.value))
                {
                    dir = dir.normalized * hit.distance;
                }

                vertices.Add(transform.InverseTransformDirection(dir));
                normals.Add(Vector3.up);
                uv.Add(Vector2.zero);

                if (index > 1)
                {
                    triangles.Add(0);
                    triangles.Add(index - 1);
                    triangles.Add(index);
                }

                index++;
            }

            Mesh m = mesh.sharedMesh;
            m.Clear();
            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);
            m.SetNormals(normals);
            m.SetUVs(0, uv);
        }
    }
}
