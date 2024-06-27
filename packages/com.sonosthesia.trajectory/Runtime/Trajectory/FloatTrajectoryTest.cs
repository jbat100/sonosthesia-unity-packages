using Sonosthesia.Ease;
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
            if(GUILayout.Button("Grid"))
            {
                test.Grid();
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

        [SerializeField] private float _duration;
        
        [Header("Trajectory")]
        
        [SerializeField] private float _targetPosition;
        [SerializeField] private float _targetVelocity;
        
        [Header("Ease")]
        
        [SerializeField] private EaseType _velocityEaseType;
        [SerializeField] private float _velocityEaseTarget;
        
        [Header("Grid")]
        
        [SerializeField] private float _gridSize;
        [SerializeField] private float _gridJump;
        [SerializeField] private float _gridVelocity;
        [SerializeField] private FloatTrajectory.GridSnap _gridSnap;
        
        
        protected virtual void Awake()
        {
            _trajectory = GetComponent<FloatTrajectory>();
        }

        public void Stop()
        {
            _trajectory.TriggerImmediate(0, 0);
        }

        public void Play()
        {
            _trajectory.TriggerBounded(_duration, _targetPosition, _targetVelocity);
        }
        
        public void Grid()
        {
            _trajectory.TriggerGridBounded(_duration, _gridJump, _gridSize, _gridVelocity, _gridSnap);
        }
        
        public void Ease()
        {
            _trajectory.TriggerVelocity(_duration, _velocityEaseType, _velocityEaseTarget);
        }
        
        public void Origin()
        {
            _trajectory.TriggerBounded(_duration, 0, 0);
        }
    }
}