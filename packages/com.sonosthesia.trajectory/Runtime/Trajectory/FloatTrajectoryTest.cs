using UnityEngine;

namespace Sonosthesia.Trajectory
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(FloatTrajectoryTest))]
    public class FloatTrajectoryTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FloatTrajectoryTest test = (FloatTrajectoryTest)target;
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
    
    [RequireComponent(typeof(FloatTrajectory))]
    public class FloatTrajectoryTest : MonoBehaviour
    {
        [SerializeField] private FloatTrajectorySettings _settings;
        
        private FloatTrajectory _trajectory;
        
        protected virtual void Awake()
        {
            _trajectory = GetComponent<FloatTrajectory>();
        }

        public void Stop()
        {
            _trajectory.TriggerImmediate(0, 0);
        }

        public void Trigger()
        {
            _settings.Trigger(_trajectory);
        }
    }
}