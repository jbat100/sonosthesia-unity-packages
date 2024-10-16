using Sonosthesia.Utils;
using UnityEngine;
using Unity.Mathematics;

namespace Sonosthesia.Generator
{
    public class BrownianRotationGenerator : BrownianGenerator<Quaternion>
    {
        private float3 _offset;

        public override Quaternion Evaluate(float time)
        {
            float3 np = math.float3(
                MathExtensions.FractalBrownianMotion(_offset.x, time, Octaves),
                MathExtensions.FractalBrownianMotion(_offset.y, time, Octaves),
                MathExtensions.FractalBrownianMotion(_offset.z, time, Octaves)
            );
            
            return quaternion.EulerZXY(math.radians(np * Amount / 0.75f));
        }

        protected override void Rehash(uint seed)
        {
            var rand = MathExtensions.Random(seed);
            _offset = rand.NextFloat3(-1e3f, 1e3f);
        }
    }
}