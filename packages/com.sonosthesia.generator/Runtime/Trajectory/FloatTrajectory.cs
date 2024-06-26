using System;
using Sonosthesia.Ease;
using Unity.Mathematics;

namespace Sonosthesia.Generator
{
    
    public class FloatTrajectory : ValueTrajectory<float>
    {
        protected override Func<float, float, float, float> LerpingFunction => math.lerp;

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