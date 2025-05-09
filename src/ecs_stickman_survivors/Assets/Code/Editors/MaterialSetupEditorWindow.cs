using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Editors
{
    public class MaterialSetupEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Material Setup Tool")]
        private static void OpenWindow()
        {
            GetWindow<MaterialSetupEditorWindow>().Show();
        }

        [BoxGroup("Set Material To Children")] 
        [LabelText("Target Material")]
        private Material _targetMaterial;

        [BoxGroup("Set Material To Children")] 
        [LabelText("Target Prefab or Scene Object")]
        [SerializeField]
        private GameObject _rootObject;

        [BoxGroup("Set Material To Children")] 
        [Space] 
        [LabelText("Override All Material Slots")]
        [SerializeField]
        private bool _overrideAllSlots = true;

        [FormerlySerializedAs("TargetMaterialIndex")]
        [BoxGroup("Set Material To Children")]
        [ShowIf("@!_overrideAllSlots")]
        [LabelText("Target Material Slot Index")]
        [MinValue(0)]
        [SerializeField]
        private int _targetMaterialIndex = 0;

        [BoxGroup("Set Material To Children")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.8f, 1f)]
        private void ApplyMaterialToChildren()
        {
            if (_targetMaterial == null || _rootObject == null)
            {
                Debug.LogError("Please assign both a material and a root object.");
                return;
            }

            int count = 0;
            MeshRenderer[] meshRenderers = _rootObject.GetComponentsInChildren<MeshRenderer>(true);
            SkinnedMeshRenderer[] skinnedRenderers = _rootObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (Renderer renderer in meshRenderers.Cast<Renderer>().Concat(skinnedRenderers))
            {
                Material[] mats = renderer.sharedMaterials;

                if (_overrideAllSlots)
                {
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = _targetMaterial;
                    }
                }
                else if (_targetMaterialIndex < mats.Length)
                {
                    mats[_targetMaterialIndex] = _targetMaterial;
                }

                renderer.sharedMaterials = mats;
                count++;
            }

            Debug.Log($"Replaced materials in {count} renderers under '{_rootObject.name}'.");
        }

        [BoxGroup("Set Random Material for Matching Mesh")]
        [LabelText("Mesh Name Contains")]
        [SerializeField]
        private string _meshNameContains;
        
        [BoxGroup("Set Random Material for Matching Mesh")]
        [LabelText("Root Object")]
        [SerializeField]
        private GameObject _rootForMeshSearch;
        
        [BoxGroup("Set Random Material for Matching Mesh")]
        [LabelText("Possible Materials")]
        public Material[] _materialsToApply;
        
        [BoxGroup("Set Random Material for Matching Mesh")]
        [Button(ButtonSizes.Large)]
        [GUIColor(0.6f, 1f, 0.6f)]
        private void ApplyRandomMaterialToMatchingMeshNames()
        {
            if (string.IsNullOrWhiteSpace(_meshNameContains) || _rootForMeshSearch == null || 
                _materialsToApply == null || _materialsToApply.Length == 0)
            {
                Debug.LogError("Please assign a mesh name, root object, and at least one material.");
                return;
            }

            int count = 0;
            MeshFilter[] meshFilters = _rootForMeshSearch.GetComponentsInChildren<MeshFilter>(true);
            SkinnedMeshRenderer[] skinnedMeshes = _rootForMeshSearch.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh == null || !meshFilter.sharedMesh.name.Contains(_meshNameContains)) 
                    continue;
                
                Renderer renderer = meshFilter.GetComponent<Renderer>();
                if (renderer == null) 
                    continue;
                
                ApplyRandomMaterialToRenderer(renderer);
                count++;
            }

            foreach (var skinnedMeshRenderer in skinnedMeshes)
            {
                if (skinnedMeshRenderer.sharedMesh == null || 
                    !skinnedMeshRenderer.sharedMesh.name.Contains(_meshNameContains)) 
                    continue;
                
                ApplyRandomMaterialToRenderer(skinnedMeshRenderer);
                count++;
            }

            Debug.Log($"Replaced materials in {count} renderers where mesh name contains '{_meshNameContains}'.");
        }

        private void ApplyRandomMaterialToRenderer(Renderer renderer)
        {
            Material randomMaterial = _materialsToApply[Random.Range(0, _materialsToApply.Length)];
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
                materials[i] = randomMaterial;

            renderer.sharedMaterials = materials;
        }
    }
}