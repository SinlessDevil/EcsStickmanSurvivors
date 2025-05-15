using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Code.Editors.ColliderMeshCreator
{
    [CustomEditor(typeof(ManualOutlineDrawer))]
    public class ManualOutlineDrawerEditor : OdinEditor
    {
        private ManualOutlineDrawer _drawer;

        private void OnEnable()
        {
            _drawer = (ManualOutlineDrawer)target;
        }

        private void OnSceneGUI()
        {
            if (_drawer.Points == null)
                return;

            Undo.RecordObject(_drawer, "Move Outline Point");

            for (int i = 0; i < _drawer.Points.Count; i++)
            {
                Vector3 localPoint = _drawer.Points[i];
                Vector3 worldPoint = _drawer.transform.TransformPoint(localPoint);

                Vector3 newWorldPoint = Handles.PositionHandle(worldPoint, Quaternion.identity);
                if (worldPoint != newWorldPoint)
                {
                    _drawer.Points[i] = _drawer.transform.InverseTransformPoint(newWorldPoint);
                    EditorUtility.SetDirty(_drawer);
                }
            }
        }
    }   
}