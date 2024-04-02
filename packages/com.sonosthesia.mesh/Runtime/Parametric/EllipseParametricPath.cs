using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public class EllipseParametricPath : ParametricPath
    {
        [SerializeField] private float _a = 1f;
        
        [SerializeField] private float _b = 2f;

        // formulas for ellipse perimeters are non exact and silly https://www.cuemath.com/measurement/perimeter-of-ellipse/
        public override float GetLength() => Mathf.PI * (3f * (_a + _b) - math.sqrt((3f * _a + _b) * (_a + 3f * _b)));

        public override bool Populate(ref NativeArray<RigidTransform> points, float2 range, int segments)
        {
            for (int index = 0; index < segments; ++index)
            {
                float s = index / (segments - 1f);
                float t = math.lerp(range.x, range.y, s) * TAU;
                float3 position = new float3(_a * math.cos(t), 0f, _b * math.sin(t));
                points[index] = new RigidTransform(quaternion.identity, position);
            }
            
            RecalculateRotations(ref points, range);
            
            return true;
        }
    }
}