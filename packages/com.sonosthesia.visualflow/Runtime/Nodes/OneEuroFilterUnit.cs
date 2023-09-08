using Sonosthesia.Utils;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.VisualFlow
{
    [UnitCategory("Sonosthesia")]
    [UnitTitle("One Euro Filter")]
    public class OneEuroFilterUnit : Unit
    {
        [DoNotSerialize]
        public ValueInput value;

        [DoNotSerialize]
        public ValueInput beta;
        
        [DoNotSerialize]
        public ValueInput minCutoff;
        
        [DoNotSerialize]
        public ValueOutput result;

        private readonly OneEuroFilter2 _filter = new ();
        
        protected override void Definition()
        {
            value = ValueInput<float>("value", 0);
            beta = ValueInput<float>("beta", 0);
            minCutoff = ValueInput<float>("minCutoff", 0);
            result = ValueOutput<float>("result", (flow) =>
            {
                _filter.Beta = flow.GetValue<float>(beta);
                _filter.MinCutoff = flow.GetValue<float>(minCutoff);
                return _filter.Step(Time.time, new float2(flow.GetValue<float>(value), 0f)).x;
            });
        }
    }
}