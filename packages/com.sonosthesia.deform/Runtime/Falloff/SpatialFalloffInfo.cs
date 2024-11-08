using System.Runtime.CompilerServices;
using Sonosthesia.Ease;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;

namespace Sonosthesia.Deform
{
    // consider further distance shapes https://iquilezles.org/articles/distfunctions/
    
    public enum SpatialFalloffShape
    {
        Spherical,
        Capsule,
        Planar
    }

    public readonly struct SpatialFalloffInfo
    {
        public readonly bool active;
        public readonly SpatialFalloffShape shape;
        public readonly EaseType ease;
        public readonly float3 center;
        public readonly float3 handle;
        public readonly float radius;

        public SpatialFalloffInfo(bool active, SpatialFalloffShape shape, EaseType ease, float3 center, float3 handle, float radius)
        {
            this.active = active;
            this.shape = shape;
            this.ease = ease;
            this.center = center;
            this.handle = handle;
            this.radius = radius;
        }
        
        public override string ToString()
        {
            return $"{nameof(SpatialFalloffInfo)} " +
                   $"{nameof(active)}: {active}, " +
                   $"{nameof(shape)}: {shape}, " +
                   $"{nameof(ease)}: {ease}, " +
                   $"{nameof(center)}: {center}, " +
                   $"{nameof(handle)}: {handle}, " +
                   $"{nameof(radius)}: {radius}";
        }
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
    public struct SpatialFalloffCompute
    {
        private const float LENGTH_SQR_THRESHOLD = 1e-6f; 
        private const float RANGE_THRESHOLD = 1e-3f;

        private readonly SpatialFalloffShape shape;
        private readonly float3 center;
        
        private readonly bool zero;
        private readonly float radiusSqr;
        private readonly float radiusInverse;
        private readonly float3 direction;
        private readonly float lengthSqr;
        private Plane plane;

        public SpatialFalloffCompute(SpatialFalloffShape shape, float3 center, float3 handle, float radius)
        {
            this.shape = shape;
            this.center = center;
            
            zero = default;
            radiusSqr = default;
            plane = default;
            direction = default;
            radiusInverse = default;
            lengthSqr = default;
            if (radius < RANGE_THRESHOLD)
            {
                zero = true;
                return;
            }
            radiusInverse = 1f / radius;
            radiusSqr = math.square(radius);   
            direction = handle - center;
            lengthSqr = math.lengthsq(direction);
            if (lengthSqr > LENGTH_SQR_THRESHOLD)
            {
                plane = new Plane(direction, center);                
            }
            else
            {
                zero = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Compute(float3 point)
        {
            if (zero)
            {
                return 0f;
            }
            
            return 1f - math.clamp(Distance(point) * radiusInverse, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Distance(float3 point)
        {
            switch (shape)
            {
                case SpatialFalloffShape.Spherical:
                {
                    return math.distance(point, center);
                }
                case SpatialFalloffShape.Capsule:
                {
                    float3 pointToCenter = point - center;
                    float t = math.clamp(math.dot(pointToCenter, direction) / lengthSqr, 0f, 1f);
                    float3 q = center + t * direction;
                    return math.distance(q, point);
                }
                case SpatialFalloffShape.Planar:
                {
                    return math.abs(plane.SignedDistanceToPoint(point));
                }
            }

            return 1f;
        }

    }
}