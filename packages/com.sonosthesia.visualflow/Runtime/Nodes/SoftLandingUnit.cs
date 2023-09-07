using Unity.VisualScripting;
using UnityEngine;

namespace Sonosthesia.VisualFlow
{
    [UnitCategory("Sonosthesia")]
    [UnitTitle("Soft Landing")]
    public class SoftLandingUnit : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;
        
        [DoNotSerialize]
        public ControlOutput outputTrigger;
        
        [DoNotSerialize]
        public ValueInput value;

        [DoNotSerialize]
        public ValueInput landingSpeed;

        [DoNotSerialize]
        public ValueOutput result;

        private float _target;
        private float _current;
        
        protected override void Definition()
        {
            inputTrigger = ControlInput("inputTrigger", (flow) =>
            {
                _target = flow.GetValue<float>(value);
                _current = _current > _target ? 
                    Mathf.MoveTowards(_current, _target, Time.deltaTime * flow.GetValue<float>(landingSpeed)) : 
                    _target;
                return outputTrigger;
            });
            outputTrigger = ControlOutput("outputTrigger");
            value = ValueInput<float>("value", 0);
            landingSpeed = ValueInput<float>("landingSpeed", 0);
            result = ValueOutput<float>("result", (flow) => _current);
        }
    }
}