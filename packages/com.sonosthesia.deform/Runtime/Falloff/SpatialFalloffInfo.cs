using System.Runtime.CompilerServices;
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

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
    public struct SpatialFalloffInfo
    {
        public const float LENGTH_THRESHOLD = 1e-3f; 
        public const float RANGE_THRESHOLD = 1e-3f; 
        
        public readonly bool active;
        public readonly SpatialFalloffShape shape;
        public readonly float3 center;
        public readonly float3 handle;
        public readonly float radius;

        // --------- precomputed -----------
        
        private readonly bool zero;
        private readonly float radiusSqr;
        private readonly float radiusInverse;
        private Plane plane;
        private readonly float3 direction;
        private readonly float length;
        private readonly float lengthSqr;
        private readonly float3 directionNormalized;

        public SpatialFalloffInfo(bool active, SpatialFalloffShape shape, float3 center, float3 handle, float radius)
        {
            this.active = active;
            this.shape = shape;
            this.center = center;
            this.handle = handle;
            this.radius = radius;

            zero = default;
            radiusSqr = default;
            plane = default;
            direction = default;
            length = default;
            directionNormalized = default;
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
            length = math.rsqrt(lengthSqr);
            if (length > LENGTH_THRESHOLD)
            {
                plane = new Plane(direction, center);                
            }
            else
            {
                zero = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Apply(float3 point, float value)
        {
            if (!active)
            {
                return value;
            }

            if (zero)
            {
                return 0f;
            }
            
            return active ? Distance(point) * radiusInverse * value : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Distance(float3 point)
        {
            switch (shape)
            {
                case SpatialFalloffShape.Spherical:
                {
                    float distanceSqr = math.distancesq(point, center);
                    if (distanceSqr > radiusSqr)
                    {
                        return 0;
                    }
                    float distance = math.rsqrt(distanceSqr);
                    return distance;
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