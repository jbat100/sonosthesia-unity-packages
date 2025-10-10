using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using Unity.Mathematics;
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
            Twist
        }

        public enum DistanceType
        {
            ActorToSource,
            ActorRelative,
            ActorComponent,
            Height
        }

        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;

        // ----------- velocity -------------
        
        [SerializeField] private TouchVelocityType _velocityType = TouchVelocityType.Actor;

        // ----------- distance -------------
        
        [SerializeField] private DistanceType _distanceType = DistanceType.ActorToSource;
        [SerializeField] private Axes _distanceAxes = Axes.X | Axes.Y | Axes.Z;
        [SerializeField] private bool _normalizedDistance;

        // ----------- actor modulation -------------

        [SerializeField] private TouchActorModulationType _actorModulationType;
        [SerializeField] private FloatModulationSettings _actorModulation;

        protected override bool BypassFollow => _extractorType == ExtractorType.Constant;

        protected override IDynamicExtractorSession<TouchEvent, float> MakeRawSession()
        {
            IDynamicExtractorSession<TouchEvent, float> DistanceSession()
            {
                return _distanceType switch
                {
                    DistanceType.ActorToSource => new ActorToSourceDistanceSession(_distanceAxes, _normalizedDistance),
                    DistanceType.ActorRelative => new ActorRelativeDistanceSession(_distanceAxes),
                    DistanceType.ActorComponent => new ActorComponentDistanceSession(_distanceAxes, _normalizedDistance),
                    DistanceType.Height => new HeightDistanceSession(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            IDynamicExtractorSession<TouchEvent, float> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Constant => ConstantSession(),
                ExtractorType.Velocity => new VelocitySession(_velocityType),
                ExtractorType.Distance => DistanceSession(),
                ExtractorType.Twist => new TwistSession(),
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
        
        private class ActorRelativeDistanceSession : IDynamicExtractorSession<TouchEvent, float>
        {
            private readonly Axes _axes;

            private Vector3 _actorPositionReference;

            public ActorRelativeDistanceSession(Axes axes)
            {
                _axes = axes;
            }

            private bool Common(TouchEvent touchEvent, out float value)
            {
                Vector3 actorPosition = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes);
                value = (_actorPositionReference - actorPosition).magnitude;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _actorPositionReference = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes);
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
        
        private class ActorToSourceDistanceSession : IDynamicExtractorSession<TouchEvent, float>
        {
            private readonly bool _normalized;
            private readonly Axes _axes;

            private float _referenceDistance;

            public ActorToSourceDistanceSession(Axes axes, bool normalized)
            {
                _normalized = normalized;
                _axes = axes;
            }

            private bool Common(TouchEvent touchEvent, out float value)
            {
                float distance = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes).magnitude;
                value = _normalized ? distance / _referenceDistance : distance;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _referenceDistance = math.max(touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes).magnitude, 1e-3f);
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
        
        private class ActorComponentDistanceSession : IDynamicExtractorSession<TouchEvent, float>
        {
            private readonly bool _normalized;
            private readonly Axes _axes;

            private float _referenceDistance;

            public ActorComponentDistanceSession(Axes axes, bool normalized)
            {
                _normalized = normalized;
                _axes = axes;
            }

            private bool Common(TouchEvent touchEvent, out float value)
            {
                float distance = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes).Sum();
                value = _normalized ? distance / _referenceDistance : distance;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _referenceDistance = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes).Sum();
                if (math.abs(_referenceDistance) < 1e-3f)
                {
                    _referenceDistance = 1e-3f;
                }
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
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