using System;
using UnityEngine;
using Sonosthesia.Ease;
using Sonosthesia.Signal;

namespace Sonosthesia.Trajectory
{
    public abstract class ValueTrajectory<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _positionTarget;
        
        [SerializeField] private Signal<T> _velocityTarget;

        public T Position => _currentState.Position;
        public T Velocity => _currentState.Velocity;
        
        protected abstract Func<T, T, float, T> LerpingFunction { get; }
        
        private readonly struct State
        {
            public readonly T Position;
            public readonly T Velocity;

            public State(T position, T velocity)
            {
                Position = position;
                Velocity = velocity;
            }
        }

        private State _currentState = default;
        
        // note: either one or the other, if both are set _trajectory is used
        private ITrajectory<T> _trajectory;
        private ICurve<T> _velocityCurve; 

        public void TriggerImmediate(T position, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;
            
            _currentState = new State(position, velocity);
        }

        public void TriggerVelocity(float duration, EaseType easeType, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;

            if (duration <= 0)
            {
                _currentState = new State(_currentState.Position, velocity);
                return;
            }
            
            CurveEndpoint<T> start = new CurveEndpoint<T>(Time.time, _currentState.Velocity);
            CurveEndpoint<T> end = new CurveEndpoint<T>(Time.time + duration, velocity);
            _velocityCurve = new EaseCurve<T>(easeType, start, end, LerpingFunction);
        }

        public void TriggerPulse(float duration, EaseType easeType, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;

            if (duration <= 0)
            {
                _currentState = new State(_currentState.Position, velocity);
                return;
            }
            
            CurveEndpoint<T> start = new CurveEndpoint<T>(Time.time, _currentState.Velocity);
            CurveEndpoint<T> middle = new CurveEndpoint<T>(Time.time + duration * 0.5f, velocity);
            CurveEndpoint<T> end = new CurveEndpoint<T>(Time.time + duration , _currentState.Velocity);
            _velocityCurve = new PulseCurve<T>(easeType, start, middle, end, LerpingFunction);
        }
        
        public void TriggerBounded(float duration, T position, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;
            
            if (duration <= 0f)
            {
                TriggerImmediate(position, velocity);
                return;
            }

            TrajectoryBoundary<T> start = new TrajectoryBoundary<T>(Time.time, _currentState.Position, _currentState.Velocity);
            TrajectoryBoundary<T> end = new TrajectoryBoundary<T>(Time.time + duration, position, velocity);
            
            _trajectory = CreateTrajectory(start, end);
            // Debug.LogWarning($"{this} started trajectory with value start {start} end {end}");
        }

        protected abstract ITrajectory<T> CreateTrajectory(TrajectoryBoundary<T> start, TrajectoryBoundary<T> end);

        protected abstract T UpdateState(T current, T velocity, float deltaTime);

        protected virtual void Update()
        {
            if (_trajectory != null)
            {
                if (_trajectory.End.Time <= Time.time)
                {
                    float deltaTime = Time.time - _trajectory.End.Time;
                    T updated = UpdateState(_trajectory.End.Position, _trajectory.End.Velocity, deltaTime);
                    // Debug.LogWarning($"{this} ended trajectory at time {Time.time} with value {updated}");
                    BroadcastState(updated, _trajectory.End.Velocity);
                    _trajectory = null;
                }
                else
                {
                    _trajectory.Evaluate(Time.time, out T position, out T velocity);
                    // Debug.Log($"{this} evaluated trajectory with value {position}");
                    BroadcastState(position, velocity);
                }
            }
            else if (_velocityCurve != null)
            {
                if (_velocityCurve.End.Time <= Time.time)
                {
                    float deltaTime = Time.time - _velocityCurve.End.Time;
                    T updated = UpdateState(_currentState.Position, _velocityCurve.End.Value, deltaTime);
                    BroadcastState(updated, _velocityCurve.End.Value);
                    _velocityCurve = null;
                }
                else
                {
                    _velocityCurve.Evaluate(Time.time, out T velocity);
                    T updated = UpdateState(_currentState.Position, velocity, Time.deltaTime);
                    // Debug.Log($"{this} evaluated trajectory with value {position}");
                    BroadcastState(updated, velocity);
                }
            }
            else
            {
                T updated = UpdateState(_currentState.Position, _currentState.Velocity, Time.deltaTime);
                // Debug.Log($"{this} applied velocity {_currentState.Velocity} updated position {updated}");
                BroadcastState(updated, _currentState.Velocity);
            }
        }

        private void BroadcastState(T position, T velocity)
        {
            _currentState = new State(position, velocity);
            if (_positionTarget)
            {
                _positionTarget.Broadcast(position);
            }

            if (_velocityTarget)
            {
                _velocityTarget.Broadcast(velocity);
            }
        }
    }
}