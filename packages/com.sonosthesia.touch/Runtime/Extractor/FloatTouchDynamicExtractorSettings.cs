using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchDynamicExtractorSettings : FloatDynamicExtractorSettings<TouchEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Velocity,
            Distance,
            Twist,
            Height
        }

        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;
        
        [SerializeField] private TouchVelocityType _velocityType = TouchVelocityType.Actor;

        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;
        
        [SerializeField] private TouchActorModulationType _actorModulationType;
        [SerializeField] private FloatModulationSettings _actorModulation;

        protected override bool BypassFollow => _extractorType == ExtractorType.Constant;

        protected override IDynamicExtractorSession<TouchEvent, float> MakeRawSession()
        {
            IDynamicExtractorSession<TouchEvent, float> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Constant => ConstantSession(),
                ExtractorType.Velocity => new VelocitySession(_velocityType),
                ExtractorType.Distance => new ActorToSourceDistanceSession(_axes),
                ExtractorType.Twist => new TwistSession(),
                ExtractorType.Height => new HeightDistanceSession(),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (_actorModulationType != TouchActorModulationType.None)
            {
                session = new ActorModulationSession(session, _actorModulationType, _actorModulation);
            }

            return session;
        }

        private class ActorModulationSession : ExtractorSessionProcessor<TouchEvent, float>
        {
            private readonly TouchActorModulationType _type;
            private readonly FloatModulationSettings _settings;
            
            public ActorModulationSession(IDynamicExtractorSession<TouchEvent, float> session, TouchActorModulationType type, FloatModulationSettings settings) 
                : base(session)
            {
                _type = type;
                _settings = settings;
            }

            protected override float Process(TouchEvent touchEvent, float value)
            {
                TouchActorModulator modulator = touchEvent.touchData.Actor.Modulator;
                return _settings.Modulate(modulator ? modulator.Select(_type) : 0f, value);
            }
        }

        private class VelocitySession : StatelessExtractorSession<TouchEvent, float>
        {
            private readonly TouchVelocityType _type;
            
            public VelocitySession(TouchVelocityType type)
            {
                _type = type;
            }

            protected override bool Extract(TouchEvent touchEvent, out float value)
            {
                return TouchExtractionUtils.ExtractVelocity(touchEvent, _type, out value);
            }
        }
        
        private class ActorToSourceDistanceSession : StatelessExtractorSession<TouchEvent, float>
        {
            private readonly Axes _axes;
            
            public ActorToSourceDistanceSession(Axes axes)
            {
                _axes = axes;
            }

            protected override bool Extract(TouchEvent e, out float value)
            {
                value = e.ActorPositionInSourceSpace().FilterAxes(_axes).magnitude;
                return true;
            }
        }

        private class HeightDistanceSession : StatelessExtractorSession<TouchEvent, float>
        {
            protected override bool Extract(TouchEvent e, out float value)
            {
                value = e.touchData.Actor.transform.position.y;
                return true;
            }
        }

        private class TwistSession : IDynamicExtractorSession<TouchEvent, float>
        {
            private Quaternion _referenceRotation;

            private bool Common(TouchEvent touchEvent, out float value)
            {
                Quaternion rotation = touchEvent.touchData.Actor.transform.rotation;
                value = Quaternion.Angle(_referenceRotation, rotation) / 180f;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _referenceRotation = touchEvent.touchData.Actor.transform.rotation;
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
        
        private class BinSession : ExtractorSessionProcessor<TouchEvent, float>
        {
            private readonly AnimationCurve _curve;
            
            public BinSession(IDynamicExtractorSession<TouchEvent, float> session, AnimationCurve curve) : base(session)
            {
                _curve = curve;
            }

            protected override float Process(TouchEvent touchEvent, float value) => _curve.Evaluate(value);
        }
    }
}