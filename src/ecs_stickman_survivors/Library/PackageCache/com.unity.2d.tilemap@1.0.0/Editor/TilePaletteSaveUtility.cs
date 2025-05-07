using System;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal class TilePaletteSaveUtility
    {
        private class TilePaletteSaveScope : IDisposable
        {
            private GameObject m_GameObject;

            public TilePaletteSaveScope(GameObject paletteInstance)
            {
                m_GameObject = paletteInstance;
                if (m_GameObject != null)
                {
                    GridPaintingState.savingPalette = true;
                    SetHideFlagsRecursively(paletteInstance, HideFlags.HideInHierarchy);
                    var renderers = paletteInstance.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                        renderer.gameObject.layer = 0;
                }
            }

            public void Dispose()
            {
                if (m_GameObject != null)
                {
                    SetHideFlagsRecursively(m_GameObject, HideFlags.HideAndDontSave);
                    GridPaintingState.savingPalette = false;
                }
            }

            private void SetHideFlagsRecursively(GameObject root, HideFlags flags)
            {
                root.hideFlags = flags;
                for (int i = 0; i < root.transform.childCount; i++)
                    SetHideFlagsRecursively(root.transform.GetChild(i).gameObject, flags);
            }
        }

        public static bool SaveTilePalette(GameObject originalPalette, GameObject paletteInstance)
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(originalPalette);
            if (path == null)
                return false;

            using (new TilePaletteSaveScope(paletteInstance))
            {
                PrefabUtility.SaveAsPrefabAssetAndConnect(paletteInstance, path, InteractionMode.AutomatedAction);
            }
            return true;
        }
    }
}
