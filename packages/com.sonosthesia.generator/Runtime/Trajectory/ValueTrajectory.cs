using Sonosthesia.Ease;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Generator
{
    public abstract class ValueTrajectory<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _positionTarget;
        
        [SerializeField] private Signal<T> _velocityTarget;
        
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

        public void SetState(T position, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;
            
            _currentState = new State(position, velocity);
        }

        public void SetVelocity(float duration, EaseType easeType, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;

            if (duration <= 0)
            {
                _currentState = new State(_currentState.Position, velocity);
                return;
            }
            
            
        }
        
        public void SetTrajectory(float duration, T position, T velocity)
        {
            _trajectory = null;
            _velocityCurve = null;
            
            if (duration <= 0f)
            {
                SetState(position, velocity);
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
                if (_trajectory.EndTime <= Time.time)
                {
                    float deltaTime = Time.time - _trajectory.EndTime;
                    if (_trajectory is IBoundaryTrajectory<T> boundaryTrajectory)
                    {
                        T updated = UpdateState(boundaryTrajectory.End.Position, boundaryTrajectory.End.Velocity, deltaTime);
                        // Debug.LogWarning($"{this} ended trajectory at time {Time.time} with value {updated}");
                        BroadcastState(updated, boundaryTrajectory.End.Velocity);    
                    }
                    
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