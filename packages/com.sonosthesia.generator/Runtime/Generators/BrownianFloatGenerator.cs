using Sonosthesia.Utils;

namespace Sonosthesia.Generator
{
    public class BrownianFloatGenerator : BrownianGenerator<float>
    {
        private float _offset;
        
        public override float Evaluate(float time)
        {
            float np = MathExtensions.FractalBrownianMotion(_offset, time, Octaves);
            return np * Amount / 0.75f;
        }

        protected override void Rehash(uint seed)
        {
            Unity.Mathematics.Random rand = MathExtensions.Random(seed);
            _offset = rand.NextFloat(-1e3f, 1e3f);
        }
    }
}