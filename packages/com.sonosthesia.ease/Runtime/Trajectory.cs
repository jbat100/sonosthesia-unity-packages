using Unity.Mathematics;

namespace Sonosthesia.Ease
{
    // Allows a trajectory from position p1, velocity v1 at time t1
    // to position p2, velocity v2 at time t2 to be computed allowing
    // smooth transition from one procedural movement function to another

    public abstract class CubicPolynomialTrajectory<T> where T : struct
    {
        protected readonly struct Coefficients
        {
            public readonly float A0;
            public readonly float A1;
            public readonly float A2;
            public readonly float A3;

            public Coefficients(float a0, float a1, float a2, float a3)
            {
                A0 = a0;
                A1 = a1;
                A2 = a2;
                A3 = a3;
            }

            public void Compute(float t, out float position, out float velocity)
            {
                float t_2 = t * t;
                float t_3 = t_2 * t;
                position = A0 + A1 * t + A2 * t_2 + A3 * t_3;
                velocity = A1 + 2 * A2 * t + 3 * A3 * t_2;
            }
        }
        
        public readonly struct Boundary
        {
            public readonly float Time;
            public readonly T Position;
            public readonly T Velocity;

            public Boundary(float time, T position, T velocity)
            {
                Time = time;
                Position = position;
                Velocity = velocity;
            }
        }

        public readonly Boundary Start;
        public readonly Boundary End;
        
        public CubicPolynomialTrajectory(Boundary start, Boundary end)
        {
            Start = start;
            End = end;
        }

        public bool Evaluate(float t, out T position, out T velocity)
        {
            if (t < Start.Time || t > End.Time)
            {
                position = default;
                velocity = default;
                return false;
            }

            return Compute(t, out position, out velocity);
        }

        protected abstract bool Compute(float t, out T position, out T velocity);

        // See README for derivation
        protected Coefficients ComputeCoefficients(float t1, float p1, float v1, float t2, float p2, float v2)
        {
            float t1_2 = t1 * t1;
            float t1_3 = t1_2 * t1;
            float t2_2 = t2 * t2;
            float t2_3 = t2_2 * t2;

            float denominator = t1_3 - 3 * t1_2 * t2 + 3 * t1 * t2_2 - t2_3;

            float a0 = 3 * p1 * t1 * t2_2 - p1 * t2_3 + p2 * t1_3 - 3 * p2 * t1_2 * t2 -
                t1_3 * t2 * v2 - t1_2 * t2_2 * v1 + t1_2 * t2_2 * v2 +
                t1 * t2_3 * v1;

            float a1 = -6 * p1 * t1 * t2 + 6 * p2 * t1 * t2 + t1_3 * v2 + 2 * t1_2 * t2 * v1 +
                t1_2 * t2 * v2 - t1 * t2_2 * v1 - 2 * t1 * t2_2 * v2 - t2_3 * v1;

            float a2 = 3 * p1 * t1 + 3 * p1 * t2 - 3 * p2 * t1 - 3 * p2 * t2 - t1_2 * v1 - 2 * t1_2 * v2 -
                t1 * t2 * v1 + t1 * t2 * v2 + 2 * t2_2 * v1 + t2_2 * v2;

            float a3 = -2 * p1 + 2 * p2 + t1 * v1 + t1 * v2 - t2 * v1 - t2 * v2;

            return new Coefficients(a0 / denominator, a1 / denominator, a2 / denominator, a3 / denominator);
        }
    }
    
    public class CubicPolynomialTrajectory1D : CubicPolynomialTrajectory<float>
    {
        private readonly Coefficients _coefficients;
        
        public CubicPolynomialTrajectory1D(Boundary start, Boundary end) : base(start, end)
        {
            _coefficients = ComputeCoefficients(
                start.Time, start.Position, start.Velocity, 
                end.Time, end.Position, end.Velocity);
        }

        protected override bool Compute(float t, out float position, out float velocity)
        {
            _coefficients.Compute(t, out position, out velocity);
            return true;
        }
    }
    
    public class CubicPolynomialTrajectory2D : CubicPolynomialTrajectory<float2>
    {
        private readonly Coefficients _xCoefficients;
        private readonly Coefficients _yCoefficients;
        
        public CubicPolynomialTrajectory2D(Boundary start, Boundary end) : base(start, end)
        {
            _xCoefficients = ComputeCoefficients(
                start.Time, start.Position.x, start.Velocity.x, 
                end.Time, end.Position.x, end.Velocity.x);
            
            _yCoefficients = ComputeCoefficients(
                start.Time, start.Position.y, start.Velocity.y, 
                end.Time, end.Position.y, end.Velocity.y);
        }

        protected override bool Compute(float t, out float2 position, out float2 velocity)
        {
            _xCoefficients.Compute(t, out float px, out float vx);
            _yCoefficients.Compute(t, out float py, out float vy);
            position = new float2(px, py);
            velocity = new float2(vx, vy);
            return true;
        }
    }
    
    public class CubicPolynomialTrajectory3D : CubicPolynomialTrajectory<float3>
    {
        private readonly Coefficients _xCoefficients;
        private readonly Coefficients _yCoefficients;
        private readonly Coefficients _zCoefficients;
        
        public CubicPolynomialTrajectory3D(Boundary start, Boundary end) : base(start, end)
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

        protected override bool Compute(float t, out float3 position, out float3 velocity)
        {
            _xCoefficients.Compute(t, out float px, out float vx);
            _yCoefficients.Compute(t, out float py, out float vy);
            _yCoefficients.Compute(t, out float pz, out float vz);
            position = new float3(px, py, pz);
            velocity = new float3(vx, vy, vz);
            return true;
        }
    }
}