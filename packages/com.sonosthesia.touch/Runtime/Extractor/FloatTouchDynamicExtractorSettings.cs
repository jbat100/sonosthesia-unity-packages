using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchDynamicExtractorSettings : InteractionFloatDynamicExtractorSettings<TouchEvent>
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
        
        [SerializeField] private TouchActorModulationType _actorModulationType;
        [SerializeField] private FloatModulationSettings _actorModulation;

        protected override bool BypassFollow => _extractorType == ExtractorType.Constant;

        protected override IDynamicExtractorSession<TouchEvent, float> MakeRawSession()
        {
            IDynamicExtractorSession<TouchEvent, float> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Constant => ConstantSession(),
                ExtractorType.Velocity => VelocitySession(),
                ExtractorType.Distance => DistanceSession(),
                ExtractorType.Twist => TwistSession(),
                ExtractorType.Height => HeightSession(),
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
    }
}