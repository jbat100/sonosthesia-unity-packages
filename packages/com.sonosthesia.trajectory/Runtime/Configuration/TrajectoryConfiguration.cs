using System;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public enum TrajectoryType
    {
        None,
        Bounded,
        Easing,
        Immediate
    }
    
    /// <summary>
    /// Settings are there to inspector configuration
    /// </summary>
    
    [Serializable]
    public class TrajectorySettings<T> where T : struct
    {
        [SerializeField] private TrajectoryType _trajectoryType;
        [SerializeField] private EaseType _easeType;
        [SerializeField] private float _duration;
        [SerializeField] private T _position;
        [SerializeField] private T _velocity;

        protected float Duration => _duration;
        protected T Position => _position;
        protected T Velocity => _velocity;
        
        public void Trigger(ValueTrajectory<T> trajectory)
        {
            switch (_trajectoryType)
            {
                case TrajectoryType.Bounded:
                    TriggerBounded(trajectory);
                    break;
                case TrajectoryType.Easing:
                    TriggerEasing(trajectory);
                    break;
                case TrajectoryType.Immediate:
                    TriggerImmediate(trajectory);
                    break;
            }
        }

        protected virtual void TriggerBounded(ValueTrajectory<T> trajectory)
        {
            trajectory.TriggerBounded(_duration, _position, _velocity);
        }
        
        protected virtual void TriggerEasing(ValueTrajectory<T> trajectory)
        {
            trajectory.TriggerVelocity(_duration, _easeType, _velocity);
        }
        
        protected virtual void TriggerImmediate(ValueTrajectory<T> trajectory)
        {
            trajectory.TriggerImmediate(_position, _velocity);
        }
    }

    public class TrajectoryConfiguration<TValue, TSettings> : ScriptableObject 
        where TValue : struct where TSettings : TrajectorySettings<TValue>
    {
        [SerializeField] private TSettings _settings;

        public void Trigger(ValueTrajectory<TValue> trajectory)
        {
            _settings.Trigger(trajectory);
        }
    }
}