using Sonosthesia.Ease;

namespace Sonosthesia.Generator
{
    
    public class FloatTrajectory : Trajectory<float>
    {
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