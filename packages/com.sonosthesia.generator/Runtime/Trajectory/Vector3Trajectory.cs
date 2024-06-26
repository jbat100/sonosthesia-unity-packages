using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Generator
{
    public class Vector3Trajectory : ValueTrajectory<Vector3>
    {
        protected override ITrajectory<Vector3> CreateTrajectory(TrajectoryBoundary<Vector3> start, TrajectoryBoundary<Vector3> end)
        {
            return new CubicPolynomialTrajectoryVector3(start, end);
        }

        protected override Vector3 UpdateState(Vector3 current, Vector3 velocity, float deltaTime)
        {
            return current + velocity * deltaTime;
        }
    }
}