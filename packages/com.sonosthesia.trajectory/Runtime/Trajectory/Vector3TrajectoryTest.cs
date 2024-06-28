using UnityEngine;

namespace Sonosthesia.Trajectory
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Vector3TrajectoryTest))]
    public class Vector3TrajectoryTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Vector3TrajectoryTest test = (Vector3TrajectoryTest)target;
            if(GUILayout.Button("Trigger"))
            {
                test.Trigger();
            }
            if(GUILayout.Button("Stop"))
            {
                test.Stop();
            }
        }
    }
#endif
    
    [RequireComponent(typeof(Vector3Trajectory))]
    public class Vector3TrajectoryTest : MonoBehaviour
    {
        [SerializeField] private TrajectorySettings<Vector3> _settings;

        private Vector3Trajectory _trajectory;
        
        protected virtual void Awake()
        {
            _trajectory = GetComponent<Vector3Trajectory>();
        }
        
        public void Stop()
        {
            _trajectory.TriggerImmediate(Vector3.zero, Vector3.zero);
        }

        public void Trigger()
        {
            _settings.Trigger(_trajectory);
        }
    }
}