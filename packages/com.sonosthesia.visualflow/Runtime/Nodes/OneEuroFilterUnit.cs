using Sonosthesia.Utils;
using Unity.VisualScripting;
using Unity.Mathematics;

namespace Sonosthesia.VisualFlow
{
    [UnitCategory("Sonosthesia")]
    [UnitTitle("One Euro Filter")]
    public class OneEuroFilterUnit : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;
        
        [DoNotSerialize]
        public ControlOutput outputTrigger;
        
        [DoNotSerialize]
        public ValueInput value;

        [DoNotSerialize]
        public ValueInput time;
        
        [DoNotSerialize]
        public ValueInput beta;
        
        [DoNotSerialize]
        public ValueInput minCutoff;
        
        [DoNotSerialize]
        public ValueOutput result;

        private readonly OneEuroFilter2 _filter = new ();
        private float _filtered;
        
        protected override void Definition()
        {
            inputTrigger = ControlInput("inputTrigger", (flow) =>
            {
                _filter.Beta = flow.GetValue<float>(beta);
                _filter.MinCutoff = flow.GetValue<float>(minCutoff);
                _filtered = _filter.Step(flow.GetValue<float>(time), new float2(flow.GetValue<float>(value), 0f)).x;
                return outputTrigger;
            });
            outputTrigger = ControlOutput("outputTrigger");
            value = ValueInput<float>("value", 0);
            time = ValueInput<float>("time", 0);
            beta = ValueInput<float>("beta", 0);
            minCutoff = ValueInput<float>("minCutoff", 0);
            result = ValueOutput<float>("result", (flow) => _filtered);
        }
    }
}