using System;
using Unity.Mathematics;

namespace Sonosthesia.Trajectory
{
    public class FloatTrajectory : ValueTrajectory<float>
    {
        public enum GridSnap
        {
            Closest,
            Forward,
            Backward,
            Inverted
        }
        
        protected override Func<float, float, float, float> LerpingFunction => math.lerp;

        public void TriggerGridBounded(float duration, float jump, float size, float velocity, GridSnap gridSnap)
        {
            bool forward = Velocity >= 0;
            
            Func<float, float> snapMethod = gridSnap switch
            {
                GridSnap.Forward => math.ceil,
                GridSnap.Backward => math.floor,
                GridSnap.Inverted => forward ? math.floor : math.ceil,
                _ => math.round
            };

            float jumpDirection = gridSnap switch
            {
                GridSnap.Closest => forward ? 1 : -1,
                GridSnap.Forward => 1,
                GridSnap.Backward => -1,
                GridSnap.Inverted => forward ? -1 : 1,
                _ => jump
            };
            
            float position = snapMethod((Position + jump * jumpDirection) / size) * size;
            TriggerBounded(duration, position, velocity);
        }
        
        protected override ITrajectory<float> CreateTrajectory(TrajectoryBoundary<float> start, TrajectoryBoundary<float> end)
        {
            return new CubicPolynomialTrajectory1D(start, end);
        }

        protected override float UpdateState(float current, float velocity, float deltaTime)
        {
            return current + velocity * deltaTime;
        }
    }
}