using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public class TorusKnotParametricPath : ParametricPath
    {
        [SerializeField] private float _scale;

        [SerializeField] private int _p;
        
        [SerializeField] private int _q;

        public override float GetLength() => 1f;

        public override bool Populate(ref NativeArray<RigidTransform> points, float2 range, int segments)
        {
            for (int index = 0; index < segments; ++index)
            {
                float s = index / (segments - 1f);
                float t = math.lerp(range.x, range.y, s) * TAU;

                float r = math.cos(_q * t) + 2f;
                float x = r * math.cos(_p * t);
                float y = r * math.sin(_p * t);
                float z = - math.sin(_q * t);
                
                float3 position = _scale * (new float3(x, y, z));

                points[index] = new RigidTransform(quaternion.identity, position);
            }
            
            RecalculateRotations(ref points, range);
            
            return true;
        }
    }
}