using Sonosthesia.Utils;
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

        private SoftLanding _softLanding = new();
        
        protected override void Definition()
        {
            inputTrigger = ControlInput("inputTrigger", (flow) =>
            {
                _softLanding.Speed = flow.GetValue<float>(landingSpeed);
                _softLanding.Target = flow.GetValue<float>(value);
                _softLanding.Step(Time.deltaTime);
                return outputTrigger;
            });
            outputTrigger = ControlOutput("outputTrigger");
            value = ValueInput<float>("value", 0);
            landingSpeed = ValueInput<float>("landingSpeed", 0);
            result = ValueOutput<float>("result", (flow) => _softLanding.Current);
        }
    }
}