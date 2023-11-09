using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class OneEuroFilterOperator : SimpleOperator<float>
    {
        [SerializeField] private float _beta;

        [SerializeField] private float _minCutoff;
        
        private OneEuroFilter1 _oneEuroFilter = new ();

        protected override void OnEnable()
        {
            base.OnEnable();
            _oneEuroFilter = new OneEuroFilter1
            {
                MinCutoff = _minCutoff,
                Beta = _beta
            };
        }

        protected void OnValidate()
        {
            _oneEuroFilter.MinCutoff = _minCutoff;
            _oneEuroFilter.Beta = _beta;
        }

        protected override float Process(float input) => _oneEuroFilter.Step(Time.time, input);
    }
}