using UnityEditor;
using UnityEngine;

namespace Code.Editors.ColliderMeshCreator
{
    [CustomEditor(typeof(ManualOutlineDrawer))]
    public class ManualOutlineDrawerEditor : Editor
    {
        private const float NewPointOffset = 0.5f;
        private const string InsertKeyPrefsKey = "ColliderMesh_InsertKey";
        
        private ManualOutlineDrawer _drawer;
        private int _activeHandleIndex = -1;

        private void OnEnable()
        {
            _drawer = (ManualOutlineDrawer)target;
        }

        private void OnSceneGUI()
        {
            if (_drawer.Points == null || _drawer.Points.Count == 0)
                return;

            Undo.RecordObject(_drawer, "Edit Outline Point");

            DrawHandles();
            InsertNewPointIfRequested();
        }

        private void DrawHandles()
        {
            for (int i = 0; i < _drawer.Points.Count; i++)
            {
                Vector3 local = _drawer.Points[i];
                Vector3 world = _drawer.transform.TransformPoint(local);

                Vector3 moved = Handles.PositionHandle(world, Quaternion.identity);

                if (world != moved)
                {
                    _drawer.Points[i] = _drawer.transform.InverseTransformPoint(moved);
                    _activeHandleIndex = i;
                    EditorUtility.SetDirty(_drawer);
                }
            }
        }

        private void InsertNewPointIfRequested()
        {
            Event e = Event.current;

            if (e.type != EventType.KeyDown || e.keyCode != InsertKey)
                return;

            e.Use();

            int insertIndex = (_activeHandleIndex >= 0 && _activeHandleIndex < _drawer.Points.Count)
                ? _activeHandleIndex + 1
                : _drawer.Points.Count;

            Vector3 basePoint = insertIndex > 0 ? _drawer.Points[insertIndex - 1] : Vector3.zero;
            Vector3 newPoint = basePoint + Vector3.right * NewPointOffset;

            _drawer.Points.Insert(insertIndex, newPoint);
            _activeHandleIndex = insertIndex;

            EditorUtility.SetDirty(_drawer);
            Debug.Log($"Inserted new point at index {insertIndex}");
        }
        
        private KeyCode InsertKey => (KeyCode)EditorPrefs.GetInt(InsertKeyPrefsKey, (int)KeyCode.Q);
    }
}