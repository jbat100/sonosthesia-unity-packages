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
        Pulse,
        Immediate
    }
    
    /// <summary>
    /// Settings are there to inspector configuration
    /// </summary>
    
    [Serializable]
    public abstract class TrajectorySettings<T> where T : struct
    {
        [SerializeField] private TrajectoryType _trajectoryType;
        [SerializeField] private EaseType _easeType;
        [SerializeField] private float _duration;
        [SerializeField] private T _position;
        [SerializeField] private T _velocity;

        protected abstract T Invert(T value);
        
        protected float Duration => _duration;
        protected T Position => _position;
        protected T Velocity => _velocity;

        protected T Process(T value, bool invert)
        {
            if (invert)
            {
                value = Invert(value);
            }

            return value;
        }

        public void Trigger(ValueTrajectory<T> trajectory, bool invert = false)
        {
            switch (_trajectoryType)
            {
                case TrajectoryType.Bounded:
                    TriggerBounded(trajectory, invert);
                    break;
                case TrajectoryType.Easing:
                    TriggerEasing(trajectory, invert);
                    break;
                case TrajectoryType.Pulse:
                    TriggerPulse(trajectory, invert);
                    break;
                case TrajectoryType.Immediate:
                    TriggerImmediate(trajectory, invert);
                    break;
            }
        }

        protected virtual void TriggerBounded(ValueTrajectory<T> trajectory, bool invert)
        {
            trajectory.TriggerBounded(_duration, _position, Process(_velocity, invert));
        }
        
        protected virtual void TriggerEasing(ValueTrajectory<T> trajectory, bool invert)
        {
            trajectory.TriggerVelocity(_duration, _easeType, Process(_velocity, invert));
        }

        protected virtual void TriggerPulse(ValueTrajectory<T> trajectory, bool invert)
        {
            trajectory.TriggerPulse(_duration, _easeType, Process(_velocity, invert));
        }
        
        protected virtual void TriggerImmediate(ValueTrajectory<T> trajectory, bool invert)
        {
            trajectory.TriggerImmediate(_position, Process(_velocity, invert));
        }
    }

    public class TrajectoryConfiguration<TValue, TSettings> : ScriptableObject 
        where TValue : struct where TSettings : TrajectorySettings<TValue>
    {
        [SerializeField] private TSettings _settings;

        public void Trigger(ValueTrajectory<TValue> trajectory, bool invert = false)
        {
            _settings.Trigger(trajectory, invert);
        }
    }
}