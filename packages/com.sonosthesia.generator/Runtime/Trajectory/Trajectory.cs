using Sonosthesia.Ease;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Generator
{
    public abstract class Trajectory<T> : MonoBehaviour where T : struct
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
        private ITrajectory<T> _currentTrajectory;
        private float _trajectoryStartTime;

        public void SetState(T position, T velocity)
        {
            _currentTrajectory = null;
            _currentState = new State(position, velocity);
        }
        
        public bool SetTrajectory(float duration, T position, T velocity)
        {
            if (duration <= 0f)
            {
                SetState(position, velocity);
                return true;
            }

            _trajectoryStartTime = Time.time;
            
            // note : as t increases, numerical errors quickly get *really* bad, so offset back to 0
            
            TrajectoryBoundary<T> start = new TrajectoryBoundary<T>(0, _currentState.Position, _currentState.Velocity);
            TrajectoryBoundary<T> end = new TrajectoryBoundary<T>(duration, position, velocity);
            
            _currentTrajectory = CreateTrajectory(start, end);
            // Debug.LogWarning($"{this} started trajectory with value start {start} end {end}");
            return true;
        }

        protected abstract ITrajectory<T> CreateTrajectory(TrajectoryBoundary<T> start, TrajectoryBoundary<T> end);

        protected abstract T UpdateState(T current, T velocity, float deltaTime);

        protected virtual void Update()
        {
            if (_currentTrajectory != null)
            {
                float elapsed = Time.time - _trajectoryStartTime;
                
                if (_currentTrajectory.End.Time <= elapsed)
                {
                    float deltaTime = elapsed - _currentTrajectory.End.Time;
                    T updated = UpdateState(_currentTrajectory.End.Position, _currentTrajectory.End.Velocity, deltaTime);
                    // Debug.LogWarning($"{this} ended trajectory at time {Time.time} with value {updated}");
                    BroadcastState(updated, _currentTrajectory.End.Velocity);
                    _currentTrajectory = null;
                }
                else
                {
                    _currentTrajectory.Evaluate(elapsed, out T position, out T velocity);
                    // Debug.Log($"{this} evaluated trajectory with value {position}");
                    BroadcastState(position, velocity);
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