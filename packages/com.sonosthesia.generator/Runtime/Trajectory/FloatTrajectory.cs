using System;
using Sonosthesia.Ease;
using Unity.Mathematics;

namespace Sonosthesia.Generator
{
    public class FloatTrajectory : ValueTrajectory<float>
    {
        protected override Func<float, float, float, float> LerpingFunction => math.lerp;

        public void SetGridTrajectory(float duration, float offset, float size, float velocity)
        {
            float position = math.round((Position + offset) / size) * size;
            SetTrajectory(duration, position, velocity);
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