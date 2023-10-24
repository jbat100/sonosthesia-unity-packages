using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class OneEuroFilterOperator : SimpleOperator<float>
    {
        [SerializeField] private float _beta;

        [SerializeField] private float _minCutoff;
        
        private OneEuroFilter2 _oneEuroFilter2 = new ();

        protected override void OnEnable()
        {
            base.OnEnable();
            _oneEuroFilter2 = new OneEuroFilter2
            {
                MinCutoff = _minCutoff,
                Beta = _beta
            };
        }

        protected void OnValidate()
        {
            _oneEuroFilter2.MinCutoff = _minCutoff;
            _oneEuroFilter2.Beta = _beta;
        }

        protected override float Process(float input) => _oneEuroFilter2.Step(Time.time, new float2(input, 0)).x;
    }
}