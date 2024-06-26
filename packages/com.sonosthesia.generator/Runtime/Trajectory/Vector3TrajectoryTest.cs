using System.Collections.Generic;
using Sonosthesia.Ease;
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
            if(GUILayout.Button("Ease"))
            {
                test.Ease();
            }
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
        [SerializeField] private List<Transform> _targets;
        [SerializeField] private float _velocity;
        [SerializeField] private float _duration;
        [SerializeField] private EaseType _velocityEaseType;
        
        private Vector3Trajectory _trajectory;
        private int _currentIndex;
        
        protected virtual void Awake()
        {
            _trajectory = GetComponent<Vector3Trajectory>();
        }
        
        public void Stop()
        {
            _trajectory.SetState(Vector3.zero, Vector3.zero);
        }

        public void Ease()
        {
            if (_targets.Count == 0)
            {
                return;
            }

            _currentIndex = (_currentIndex + 1) % _targets.Count;
            Transform target = _targets[_currentIndex];
            _trajectory.SetVelocity(_duration, _velocityEaseType, target.forward * _velocity);
        }
        
        public void Play()
        {
            if (_targets.Count == 0)
            {
                return;
            }

            _currentIndex = (_currentIndex + 1) % _targets.Count;
            Transform target = _targets[_currentIndex];
            _trajectory.SetTrajectory(_duration, target.position, target.forward * _velocity);
        }
        
        public void Origin()
        {
            _trajectory.SetTrajectory(_duration, Vector3.zero, Vector3.zero);
        }
    }
}