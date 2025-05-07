using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D.Common;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// Component responsible for managing and updating 2D IK Solvers.
    /// </summary>
    [DefaultExecutionOrder(-2)]
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    [IconAttribute(IconUtility.IconPath + "Animation.IKManager.png")]
    [ExecuteInEditMode]
    public partial class IKManager2D : MonoBehaviour, IPreviewable
    {
#if UNITY_EDITOR
        internal static event System.Action<IKManager2D> onEnabledEditor;
        internal static event System.Action<IKManager2D> onDisabledEditor;
#endif

        [SerializeField]
        List<Solver2D> m_Solvers = new List<Solver2D>();
        [SerializeField]
        [Range(0f, 1f)]
        float m_Weight = 1f;

        /// <summary>
        /// Get and set the weight for solvers.
        /// </summary>
        public float weight
        {
            get => m_Weight;
            set => m_Weight = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Get the Solvers that are managed by this manager.
        /// </summary>
        public List<Solver2D> solvers => m_Solvers;

        void OnValidate()
        {
            m_Weight = Mathf.Clamp01(m_Weight);
            OnEditorDataValidate();
        }

        void Reset()
        {
            FindChildSolvers();
            OnEditorDataValidate();
        }

        void FindChildSolvers()
        {
            m_Solvers.Clear();

            var solvers = new List<Solver2D>();
            transform.GetComponentsInChildren<Solver2D>(true, solvers);

            foreach (var solver in solvers)
            {
                if (solver.GetComponentInParent<IKManager2D>() == this)
                    AddSolver(solver);
            }
        }

        /// <summary>
        /// Add Solver to the manager.
        /// </summary>
        /// <param name="solver">Solver to add.</param>
        public void AddSolver(Solver2D solver)
        {
            if (!m_Solvers.Contains(solver))
            {
                m_Solvers.Add(solver);
                AddSolverEditorData();
            }
        }

        /// <summary>
        /// Remove Solver from the manager.
        /// </summary>
        /// <param name="solver">Solver to remove.</param>
        public void RemoveSolver(Solver2D solver)
        {
            RemoveSolverEditorData(solver);
            m_Solvers.Remove(solver);
        }

        /// <summary>
        /// Updates the Solvers in this manager.
        /// </summary>
        public void UpdateManager()
        {
            var profilerMarker = new ProfilerMarker("IKManager2D.UpdateManager");
            profilerMarker.Begin();
            foreach (var solver in m_Solvers)
            {
                if (solver == null || !solver.isActiveAndEnabled)
                    continue;

                if (!solver.isValid)
                    solver.Initialize();

                solver.UpdateIK(weight);
            }

            profilerMarker.End();
        }

        /// <summary>
        /// Used by the animation clip preview window. Recommended to not use outside of this purpose.
        /// </summary>
        public void OnPreviewUpdate()
        {
#if UNITY_EDITOR
            if (IsInGUIUpdateLoop())
                UpdateManager();
#endif
        }

        static bool IsInGUIUpdateLoop() => Event.current != null;

        void LateUpdate()
        {
            UpdateManager();
        }

#if UNITY_EDITOR
        void OnEnable()
        {
            onEnabledEditor?.Invoke(this);
        }

        void OnDisable()
        {
            onDisabledEditor?.Invoke(this);
        }

        internal static Events.UnityEvent onDrawGizmos = new Events.UnityEvent();
        void OnDrawGizmos()
        {
            onDrawGizmos.Invoke();
        }
#endif
    }
}