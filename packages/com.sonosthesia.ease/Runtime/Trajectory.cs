using UnityEngine;
using Unity.Mathematics;

namespace Sonosthesia.Ease
{
    public interface ITrajectory<T> where T : struct
    {
        void Evaluate(float t, out T position, out T velocity);
        
        float StartTime { get; }
        float EndTime { get; }
    }

    public interface IBoundaryTrajectory<T> where T : struct
    {
        public TrajectoryBoundary<T> Start { get; }
        public TrajectoryBoundary<T> End { get; }
    }
    
    public readonly struct TrajectoryBoundary<T> where T : struct
    {
        public readonly float Time;
        public readonly T Position;
        public readonly T Velocity;

        public TrajectoryBoundary(float time, T position, T velocity)
        {
            Time = time;
            Position = position;
            Velocity = velocity;
        }

        public override string ToString()
        {
            return $"<{nameof(Time)}: {Time}, {nameof(Position)}: {Position}, {nameof(Velocity)}: {Velocity}>";
        }
    }

    // Allows a trajectory from position p1, velocity v1 at time t1
    // to position p2, velocity v2 at time t2 to be computed allowing
    // smooth transition from one procedural movement function to another

    public abstract class CubicPolynomialTrajectory<T> : ITrajectory<T>, IBoundaryTrajectory<T> where T : struct
    {
        // using doubles as numerical errors quickly become noticeable with increasing time
        
        protected readonly struct Coefficients
        {
            public readonly double A0;
            public readonly double A1;
            public readonly double A2;
            public readonly double A3;

            public Coefficients(double a0, double a1, double a2, double a3)
            {
                A0 = a0;
                A1 = a1;
                A2 = a2;
                A3 = a3;
            }

            public void Compute(double t, out double position, out double velocity)
            {
                double t_2 = t * t;
                double t_3 = t_2 * t;
                position = A0 + A1 * t + A2 * t_2 + A3 * t_3;
                velocity = A1 + 2 * A2 * t + 3 * A3 * t_2;
            }
            
            public void Compute(float t, out float position, out float velocity)
            {
                Compute((double)t, out double p, out double v);
                position = (float)p;
                velocity = (float)v;
            }
        }

        private readonly TrajectoryBoundary<T> _start;
        public TrajectoryBoundary<T> Start => _start;
        public float StartTime => _start.Time;
        
        private readonly TrajectoryBoundary<T> _end;
        public TrajectoryBoundary<T> End => _end;
        public float EndTime => _end.Time;
        
        public CubicPolynomialTrajectory(TrajectoryBoundary<T> start, TrajectoryBoundary<T> end)
        {
            _start = start;
            _end = end;
        }

        public void Evaluate(float t, out T position, out T velocity)
        {
            if (t < Start.Time)
            {
                position = Start.Position;
                velocity = Start.Velocity;
            }
            else if (t > End.Time)
            {
                position = End.Position;
                velocity = End.Velocity;
            }
            else
            {
                Compute(t, out position, out velocity);   
            }
        }

        internal abstract void Compute(float t, out T position, out T velocity);

        // See README for derivation
        protected Coefficients ComputeCoefficients(double t1, double p1, double v1, double t2, double p2, double v2)
        {
            double t1_2 = t1 * t1;
            double t1_3 = t1_2 * t1;
            double t2_2 = t2 * t2;
            double t2_3 = t2_2 * t2;

            double denominator = t1_3 - 3 * t1_2 * t2 + 3 * t1 * t2_2 - t2_3;

            double a0 = 3 * p1 * t1 * t2_2 - p1 * t2_3 + p2 * t1_3 - 3 * p2 * t1_2 * t2 -
                        t1_3 * t2 * v2 - t1_2 * t2_2 * v1 + t1_2 * t2_2 * v2 +
                        t1 * t2_3 * v1;

            double a1 = -6 * p1 * t1 * t2 + 6 * p2 * t1 * t2 + t1_3 * v2 + 2 * t1_2 * t2 * v1 +
                t1_2 * t2 * v2 - t1 * t2_2 * v1 - 2 * t1 * t2_2 * v2 - t2_3 * v1;

            double a2 = 3 * p1 * t1 + 3 * p1 * t2 - 3 * p2 * t1 - 3 * p2 * t2 - t1_2 * v1 - 2 * t1_2 * v2 -
                t1 * t2 * v1 + t1 * t2 * v2 + 2 * t2_2 * v1 + t2_2 * v2;

            double a3 = -2 * p1 + 2 * p2 + t1 * v1 + t1 * v2 - t2 * v1 - t2 * v2;

            return new Coefficients(a0 / denominator, a1 / denominator, a2 / denominator, a3 / denominator);
        }
    }
    
    public class CubicPolynomialTrajectory1D : CubicPolynomialTrajectory<float>
    {
        private readonly Coefficients _coefficients;
        
        public CubicPolynomialTrajectory1D(TrajectoryBoundary<float> start, TrajectoryBoundary<float> end) : base(start, end)
        {
            _coefficients = ComputeCoefficients(
                start.Time, start.Position, start.Velocity, 
                end.Time, end.Position, end.Velocity);
        }

        internal override void Compute(float t, out float position, out float velocity)
        {
            _coefficients.Compute(t, out position, out velocity);
        }
    }
    
    public class CubicPolynomialTrajectory2D : CubicPolynomialTrajectory<float2>
    {
        private readonly Coefficients _xCoefficients;
        private readonly Coefficients _yCoefficients;
        
        public CubicPolynomialTrajectory2D(TrajectoryBoundary<float2> start, TrajectoryBoundary<float2> end) : base(start, end)
        {
            _xCoefficients = ComputeCoefficients(
                start.Time, start.Position.x, start.Velocity.x, 
                end.Time, end.Position.x, end.Velocity.x);
            
            _yCoefficients = ComputeCoefficients(
                start.Time, start.Position.y, start.Velocity.y, 
                end.Time, end.Position.y, end.Velocity.y);
        }

        internal override void Compute(float t, out float2 position, out float2 velocity)
        {
            _xCoefficients.Compute(t, out float px, out float vx);
            _yCoefficients.Compute(t, out float py, out float vy);
            position = new float2(px, py);
            velocity = new float2(vx, vy);
        }
    }
    
    public class CubicPolynomialTrajectory3D : CubicPolynomialTrajectory<float3>
    {
        private readonly Coefficients _xCoefficients;
        private readonly Coefficients _yCoefficients;
        private readonly Coefficients _zCoefficients;
        
        public CubicPolynomialTrajectory3D(TrajectoryBoundary<float3> start, TrajectoryBoundary<float3> end) : base(start, end)
        {
            _xCoefficients = ComputeCoefficients(
                start.Time, start.Position.x, start.Velocity.x, 
                end.Time, end.Position.x, end.Velocity.x);
            
            _yCoefficients = ComputeCoefficients(
                start.Time, start.Position.y, start.Velocity.y, 
                end.Time, end.Position.y, end.Velocity.y);
            
            _zCoefficients = ComputeCoefficients(
                start.Time, start.Position.z, start.Velocity.z, 
                end.Time, end.Position.z, end.Velocity.z);
        }

        internal override void Compute(float t, out float3 position, out float3 velocity)
        {
            _xCoefficients.Compute(t, out float px, out float vx);
            _yCoefficients.Compute(t, out float py, out float vy);
            _zCoefficients.Compute(t, out float pz, out float vz);
            position = new float3(px, py, pz);
            velocity = new float3(vx, vy, vz);
        }
    }
    
    public class CubicPolynomialTrajectoryVector2 : CubicPolynomialTrajectory<Vector2>
    {
        private readonly CubicPolynomialTrajectory2D _trajectory;
            
        public CubicPolynomialTrajectoryVector2(TrajectoryBoundary<Vector2> start, TrajectoryBoundary<Vector2> end) : base(start, end)
        {
            _trajectory = new CubicPolynomialTrajectory2D(
                new TrajectoryBoundary<float2>(start.Time, start.Position, start.Velocity),
                new TrajectoryBoundary<float2>(end.Time, end.Position, end.Velocity)
            );
        }

        internal override void Compute(float t, out Vector2 position, out Vector2 velocity)
        {
            _trajectory.Compute(t, out float2 p, out float2 v);
            position = p;
            velocity = v;
        }
    }
        
    public class CubicPolynomialTrajectoryVector3 : CubicPolynomialTrajectory<Vector3>
    {
        private readonly CubicPolynomialTrajectory3D _trajectory;
            
        public CubicPolynomialTrajectoryVector3(TrajectoryBoundary<Vector3> start, TrajectoryBoundary<Vector3> end) : base(start, end)
        {
            _trajectory = new CubicPolynomialTrajectory3D(
                new TrajectoryBoundary<float3>(start.Time, start.Position, start.Velocity),
                new TrajectoryBoundary<float3>(end.Time, end.Position, end.Velocity)
            );
        }

        internal override void Compute(float t, out Vector3 position, out Vector3 velocity)
        {
            _trajectory.Compute(t, out float3 p, out float3 v);
            position = p;
            velocity = v;
        }
    }
}