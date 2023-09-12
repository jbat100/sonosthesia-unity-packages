using Sonosthesia.Utils;
using UnityEngine;
using Unity.Mathematics;

namespace Sonosthesia.Flow
{
    public class BrownianPositionGenerator : Generator<Vector3>
    {
        [SerializeField] private float _amount = 1f;
        [SerializeField] private int _octaves = 2;
        [SerializeField] private uint _seed = 0;
        
        private float3 _offset;
        
        protected void Awake ()
        {
            Rehash();
        }
        
        public override Vector3 Evaluate(float time)
        {
            float3 np = math.float3(
                MathExtensions.FractalBrownianMotion(_offset.x, time, _octaves),
                MathExtensions.FractalBrownianMotion(_offset.y, time, _octaves),
                MathExtensions.FractalBrownianMotion(_offset.z, time, _octaves)
            );

            return np * _amount / 0.75f;
        }

        private void Rehash()
        {
            var rand = MathExtensions.Random(_seed);
            _offset = rand.NextFloat3(-1e3f, 1e3f);
        }
    }
}