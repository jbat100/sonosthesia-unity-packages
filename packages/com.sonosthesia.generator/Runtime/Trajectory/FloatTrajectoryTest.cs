using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Generator
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
    
    [RequireComponent(typeof(FloatTrajectory))]
    public class FloatTrajectoryTest : MonoBehaviour
    {
        private FloatTrajectory _trajectory;

        [SerializeField] private float _targetPosition;
        [SerializeField] private float _targetVelocity;
        [SerializeField] private float _duration;
        [SerializeField] private EaseType _velocityEaseType;
        
        protected virtual void Awake()
        {
            _trajectory = GetComponent<FloatTrajectory>();
        }

        public void Stop()
        {
            _trajectory.SetState(0, 0);
        }

        public void Play()
        {
            _trajectory.SetTrajectory(_duration, _targetPosition, _targetVelocity);
        }

        public void Ease()
        {
            _trajectory.SetVelocity(_duration, _velocityEaseType, _targetVelocity);
        }
        
        public void Origin()
        {
            _trajectory.SetTrajectory(_duration, 0, 0);
        }
    }
}