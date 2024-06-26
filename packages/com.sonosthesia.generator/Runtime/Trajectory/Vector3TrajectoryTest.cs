using UnityEngine;

namespace Sonosthesia.Generator
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
            if(GUILayout.Button("Play"))
            {
                test.Play();
            }
            if(GUILayout.Button("Origin"))
            {
                test.Origin();
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
        [SerializeField] private Transform _target;

        [SerializeField] private float _velocity;
        
        [SerializeField] private float _duration;
        
        private Vector3Trajectory _trajectory;

        protected virtual void Awake()
        {
            _trajectory = GetComponent<Vector3Trajectory>();
        }
        
        public void Stop()
        {
            _trajectory.SetState(Vector3.zero, Vector3.zero);
        }

        public void Play()
        {
            _trajectory.SetTrajectory(_duration, _target.position, _target.forward * _velocity);
        }
        
        public void Origin()
        {
            _trajectory.SetTrajectory(_duration, Vector3.zero, Vector3.zero);
        }
    }
}