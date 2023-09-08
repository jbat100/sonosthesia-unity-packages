using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatSoftLandingSettings : DynamicProcessorSettings
    {
        [SerializeField] private float _speed = 1f;
        public float Speed => _speed;
    }

    public class FloatSoftLandingProcessor : DynamicProcessor<float, FloatSoftLandingSettings>
    {
        public FloatSoftLandingProcessor(FloatSoftLandingSettings settings) : base(settings) { }

        private float _previousTime;
        private SoftLanding _softLanding = new ();

        protected override float Process(FloatSoftLandingSettings settings, float input, float time)
        {
            float deltaTime = time - _previousTime;
            _previousTime = time;
            _softLanding.Speed = settings.Speed;
            _softLanding.Target = input;
            return _softLanding.Step(deltaTime);
        }

        public override void Reset()
        {
            _softLanding = new SoftLanding ();
        }
    }
}