using System;
using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchExtractorSettings
    {
        public enum ExtractorType
        {
            Custom,
            Static,
            Velocity,
            Distance,
            Twist
        }

        public enum VelocityType
        {
            Actor,
            Source,
            Relative
        }

        public enum DistanceType
        {
            ActorToSource,
            ActorRelative
        }

        public enum PostProcessingType
        {
            None,
            Remap,
            Curve
        }
        
        [SerializeField] private ExtractorType _extractorType = ExtractorType.Static;

        // ----------- custom -------------
        
        [SerializeField] private TouchExtractor<float> _extractor;
        
        // ----------- static -------------
        
        [SerializeField] private float _staticValue = 1;

        // ----------- velocity -------------
        
        [SerializeField] private VelocityType _velocityType = VelocityType.Actor;

        // ----------- distance -------------
        
        [SerializeField] private DistanceType _distanceType = DistanceType.ActorToSource;
        [SerializeField] private Axes _distanceAxes = Axes.X | Axes.Y | Axes.Z;
        [SerializeField] private bool _normalizedDistance;

        // ----------- actor modulation -------------

        [SerializeField] private TouchActorModulationType _actorModulationType;
        [SerializeField] private FloatModulationSettings _actorModulation;
        
        // ----------- postprocess -------------
        
        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;
        
        // ----------- clamp -------------

        [SerializeField] private ClampSettings _clamp;

        // used when only the initial value is needed, creates a session, sets it up and returns extracted value
        public bool Extract(TouchEvent e, out float value)
        {
            ITouchExtractorSession<float> session = MakeSession();
            return session.Setup(e, out value);
        }
        
        // used when the value can change with time and may require state, such as relative distance etc...
        public ITouchExtractorSession<float> MakeSession()
        {
            ITouchExtractorSession<float> DistanceSession()
            {
                return _distanceType switch
                {
                    DistanceType.ActorToSource => new ActorToSourceDistanceSession(_distanceAxes, _normalizedDistance),
                    DistanceType.ActorRelative => new ActorRelativeDistanceSession(_distanceAxes),
                    _ => null
                };
            }
            
            ITouchExtractorSession<float> session = _extractorType switch
            {
                ExtractorType.Custom => _extractor.MakeSession(),
                ExtractorType.Static => new StaticSession(_staticValue),
                ExtractorType.Velocity => new VelocitySession(_velocityType),
                ExtractorType.Distance => DistanceSession(),
                ExtractorType.Twist => new TwistSession(),
                _ => null
            };

            if (_actorModulationType != TouchActorModulationType.None)
            {
                session = new ActorModulationSession(session, _actorModulationType, _actorModulation);
            }

            session = _postProcessing switch
            {
                PostProcessingType.Remap => new RemapSession(session, _remap),
                PostProcessingType.Curve => new CurveSession(session, _curve),
                _ => session
            };

            if (_clamp.Clamp)
            {
                session = new ClampSession(session, _clamp);
            }

            return session;
        }

        private class ActorModulationSession : TouchExtractorSessionProcessor<float>
        {
            private readonly TouchActorModulationType _type;
            private readonly FloatModulationSettings _settings;
            
            public ActorModulationSession(ITouchExtractorSession<float> session, TouchActorModulationType type, FloatModulationSettings settings) 
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
        
        private class RemapSession : TouchExtractorSessionProcessor<float>
        {
            private readonly RemapSettings _settings;
            
            public RemapSession(ITouchExtractorSession<float> session, RemapSettings settings) : base(session)
            {
                _settings = settings;
            }

            protected override float Process(TouchEvent touchEvent, float value) => _settings.Remap(value);
        }
        
        private class CurveSession : TouchExtractorSessionProcessor<float>
        {
            private readonly AnimationCurve _curve;
            
            public CurveSession(ITouchExtractorSession<float> session, AnimationCurve curve) : base(session)
            {
                _curve = curve;
            }

            protected override float Process(TouchEvent touchEvent, float value) => _curve.Evaluate(value);
        }

        private class ClampSession : TouchExtractorSessionProcessor<float>
        {
            private readonly ClampSettings _settings;
            
            public ClampSession(ITouchExtractorSession<float> session, ClampSettings settings) : base(session)
            {
                _settings = settings;
            }

            protected override float Process(TouchEvent touchEvent, float value) => _settings.Process(value);
        }

        private class StaticSession : ITouchExtractorSession<float>
        {
            private readonly float _staticValue;
            
            public StaticSession(float staticValue)
            {
                _staticValue = staticValue;
            }
            
            private bool Common(TouchEvent touchEvent, out float value)
            {
                value = _staticValue;
                return true;
            }

            public bool Setup(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }

        private class VelocitySession : ITouchExtractorSession<float>
        {
            private readonly VelocityType _type;
            
            public VelocitySession(VelocityType type)
            {
                _type = type;
            }

            private bool Common(TouchEvent touchEvent, out float value)
            {
                value = _type switch
                {
                    VelocityType.Actor => touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position.magnitude,
                    VelocityType.Source => touchEvent.touchData.Source.DynamicsMonitor.Velocity.Position.magnitude,
                    VelocityType.Relative => (touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position - 
                                              touchEvent.touchData.Source.DynamicsMonitor.Velocity.Position).magnitude,
                    _ => 0f
                };

                return true;
            }

            public bool Setup(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
        
        private class ActorRelativeDistanceSession : ITouchExtractorSession<float>
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
        
        private class ActorToSourceDistanceSession : ITouchExtractorSession<float>
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

        private class TwistSession : ITouchExtractorSession<float>
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
    }
}