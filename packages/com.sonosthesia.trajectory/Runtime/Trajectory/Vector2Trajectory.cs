using System;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public class Vector2Trajectory : ValueTrajectory<Vector2>
    {
        protected override Func<Vector2, Vector2, float, Vector2> LerpingFunction => Vector2.Lerp;

        protected override ITrajectory<Vector2> CreateTrajectory(TrajectoryBoundary<Vector2> start, TrajectoryBoundary<Vector2> end)
        {
            return new CubicPolynomialTrajectoryVector2(start, end);
        }

        protected override Vector2 UpdateState(Vector2 current, Vector2 velocity, float deltaTime)
        {
            return current + velocity * deltaTime;
        }
    }
}