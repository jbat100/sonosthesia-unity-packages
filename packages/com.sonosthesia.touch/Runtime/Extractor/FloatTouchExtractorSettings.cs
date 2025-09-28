using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchExtractorSettings : FloatExtractorSettings<TouchEvent>
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
            ActorRelative,
            ActorComponent,
            Height
        }

        [SerializeField] private ExtractorType _extractorType = ExtractorType.Static;

        // ----------- velocity -------------
        
        [SerializeField] private VelocityType _velocityType = VelocityType.Actor;

        // ----------- distance -------------
        
        [SerializeField] private DistanceType _distanceType = DistanceType.ActorToSource;
        [SerializeField] private Axes _distanceAxes = Axes.X | Axes.Y | Axes.Z;
        [SerializeField] private bool _normalizedDistance;

        // ----------- actor modulation -------------

        [SerializeField] private TouchActorModulationType _actorModulationType;
        [SerializeField] private FloatModulationSettings _actorModulation;

        // used when only the initial value is needed, creates a session, sets it up and returns extracted value
        public bool Extract(TouchEvent e, out float value)
        {
            IExtractorSession<TouchEvent, float> session = MakeSession();
            return session.Setup(e, out value);
        }
        
        // used when the value can change with time and may require state, such as relative distance etc...
        public IExtractorSession<TouchEvent, float> MakeSession()
        {
            IExtractorSession<TouchEvent, float> DistanceSession()
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
            
            IExtractorSession<TouchEvent, float> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Static => StaticSession(),
                ExtractorType.Velocity => new VelocitySession(_velocityType),
                ExtractorType.Distance => DistanceSession(),
                ExtractorType.Twist => new TwistSession(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_actorModulationType != TouchActorModulationType.None)
            {
                session = new ActorModulationSession(session, _actorModulationType, _actorModulation);
            }

            return PostProcessSession(session);
        }

        private class ActorModulationSession : ExtractorSessionProcessor<TouchEvent, float>
        {
            private readonly TouchActorModulationType _type;
            private readonly FloatModulationSettings _settings;
            
            public ActorModulationSession(IExtractorSession<TouchEvent, float> session, TouchActorModulationType type, FloatModulationSettings settings) 
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

        private class VelocitySession : IExtractorSession<TouchEvent, float>
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
        
        private class ActorRelativeDistanceSession : IExtractorSession<TouchEvent, float>
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
        
        private class ActorToSourceDistanceSession : IExtractorSession<TouchEvent, float>
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
        
        private class ActorComponentDistanceSession : IExtractorSession<TouchEvent, float>
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

        private class HeightDistanceSession : IExtractorSession<TouchEvent, float>
        {
            private float _referenceHeight;

            private bool Common(TouchEvent touchEvent, out float value)
            {
                value = touchEvent.touchData.Actor.transform.position.y - _referenceHeight;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _referenceHeight = touchEvent.touchData.Actor.transform.position.y;
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }

        private class TwistSession : IExtractorSession<TouchEvent, float>
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
            
            public BinSession(IExtractorSession<TouchEvent, float> session, AnimationCurve curve) : base(session)
            {
                _curve = curve;
            }

            protected override float Process(TouchEvent touchEvent, float value) => _curve.Evaluate(value);
        }
    }
}