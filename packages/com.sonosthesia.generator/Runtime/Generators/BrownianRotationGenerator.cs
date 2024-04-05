using Sonosthesia.Utils;
using UnityEngine;
using Unity.Mathematics;

namespace Sonosthesia.Generator
{
    public class BrownianRotationGenerator : Generator<Quaternion>
    {
        [SerializeField] private float _amount = 10f;
        [SerializeField] private int _octaves = 2;
        [SerializeField] private uint _seed = 0;
        
        private float3 _offset;
        
        protected virtual void Awake ()
        {
            Rehash();
        }
        
        public override Quaternion Evaluate(float time)
        {
            float3 np = math.float3(
                MathExtensions.FractalBrownianMotion(_offset.x, time, _octaves),
                MathExtensions.FractalBrownianMotion(_offset.y, time, _octaves),
                MathExtensions.FractalBrownianMotion(_offset.z, time, _octaves)
            );
            
            return quaternion.EulerZXY(math.radians(np * _amount / 0.75f));
        }

        private void Rehash()
        {
            var rand = MathExtensions.Random(_seed);
            _offset = rand.NextFloat3(-1e3f, 1e3f);
        }
    }
}