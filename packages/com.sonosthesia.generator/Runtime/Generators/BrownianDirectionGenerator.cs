using Sonosthesia.Utils;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Sonosthesia.Generator
{
    public class BrownianDirectionGenerator : BrownianGenerator<Vector3>
    {
        [SerializeField] private Vector3 _direction = Vector3.up;
        
        private float3 _offset;

        public override Vector3 Evaluate(float time)
        {
            float3 np = math.float3(
                MathExtensions.FractalBrownianMotion(_offset.x, time, Octaves),
                MathExtensions.FractalBrownianMotion(_offset.y, time, Octaves),
                MathExtensions.FractalBrownianMotion(_offset.z, time, Octaves)
            );
            
            return (Quaternion)quaternion.EulerZXY(math.radians(np * Amount / 0.75f)) * _direction;
        }

        protected override void Rehash(uint seed)
        {
            Random rand = MathExtensions.Random(seed);
            _offset = rand.NextFloat3(-1e3f, 1e3f);
        }
    }
}