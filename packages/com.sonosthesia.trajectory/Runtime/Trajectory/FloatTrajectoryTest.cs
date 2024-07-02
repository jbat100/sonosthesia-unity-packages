using UnityEngine;

namespace Sonosthesia.Trajectory
{
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