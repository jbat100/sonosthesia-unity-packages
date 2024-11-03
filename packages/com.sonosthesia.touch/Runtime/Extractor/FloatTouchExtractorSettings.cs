using System;
using Sonosthesia.Dynamic;
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
            Dynamic,
            Distance,
            Twist
        }

        public enum DynamicType
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

        // ----------- dynamic -------------
        
        [SerializeField] private DynamicType _dynamicType = DynamicType.Actor;
        [SerializeField] private TransformDynamics.Domain _dynamicsDomain = TransformDynamics.Domain.Position;
        [SerializeField] private TransformDynamics.Order _dynamicsOrder = TransformDynamics.Order.Velocity;

        // ----------- distance -------------
        
        [SerializeField] private DistanceType _distanceType = DistanceType.ActorToSource;
        [SerializeField] private Axes _distanceAxes = Axes.X | Axes.Y | Axes.Z;
        [SerializeField] private bool _normalizedDistance;

        // ----------- postprocess -------------
        
        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;

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
                    DistanceType.ActorToSource => new ActorToSourceDistanceTouchExtractorSession(_distanceAxes, _normalizedDistance),
                    DistanceType.ActorRelative => new ActorRelativeDistanceTouchExtractorSession(_distanceAxes),
                    _ => null
                };
            }
            
            ITouchExtractorSession<float> session = _extractorType switch
            {
                ExtractorType.Custom => _extractor.MakeSession(),
                ExtractorType.Static => new StaticFloatTouchExtractorSession(_staticValue),
                ExtractorType.Dynamic => new DynamicFloatTouchExtractorSession(_dynamicType, _dynamicsDomain, _dynamicsOrder),
                ExtractorType.Distance => DistanceSession(),
                ExtractorType.Twist => new TwistTouchExtractorSession(),
                _ => null
            };

            session = _postProcessing switch
            {
                PostProcessingType.Remap => new RemapTouchExtractorSession(session, _remap),
                PostProcessingType.Curve => new CurveTouchExtractorSession(session, _curve),
                _ => session
            };

            return session;
        }
            
        private class RemapTouchExtractorSession : TouchExtractorSessionProcessor<float, RemapSettings>
        {
            public RemapTouchExtractorSession(ITouchExtractorSession<float> session, RemapSettings processor) : base(session, processor)
            {
            }

            protected override float Process(RemapSettings processor, float value)
            {
                return processor.Remap(value);
            }
        }
        
        private class CurveTouchExtractorSession : TouchExtractorSessionProcessor<float, AnimationCurve>
        {
            public CurveTouchExtractorSession(ITouchExtractorSession<float> session, AnimationCurve processor) : base(session, processor)
            {
            }

            protected override float Process(AnimationCurve processor, float value)
            {
                return processor.Evaluate(value);
            }
        }

        private class StaticFloatTouchExtractorSession : ITouchExtractorSession<float>
        {
            private readonly float _staticValue;
            
            public StaticFloatTouchExtractorSession(float staticValue)
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

        private class DynamicFloatTouchExtractorSession : ITouchExtractorSession<float>
        {
            private readonly DynamicType _type;
            private readonly TransformDynamics.Domain _domain;
            private readonly TransformDynamics.Order _order;

            private TransformDynamicsMonitor _sourceMonitor;
            private TransformDynamicsMonitor _actorMonitor;
            
            public DynamicFloatTouchExtractorSession(DynamicType type, TransformDynamics.Domain domain, TransformDynamics.Order order)
            {
                _type = type;
                _domain = domain;
                _order = order;
            }

            private bool Common(TouchEvent touchEvent, out float value)
            {
                Vector3 source = _sourceMonitor ? _sourceMonitor.Select(_order).Select(_domain) : Vector3.zero;
                Vector3 actor = _actorMonitor ? _actorMonitor.Select(_order).Select(_domain) : Vector3.zero;

                value = _type switch
                {
                    DynamicType.Actor => actor.magnitude,
                    DynamicType.Source => source.magnitude,
                    DynamicType.Relative => (actor - source).magnitude,
                    _ => 0f
                };

                return true;
            }

            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _sourceMonitor = touchEvent.TouchData.Source.DynamicsMonitor;
                _actorMonitor = touchEvent.TouchData.Actor.DynamicsMonitor;
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
        
        private class ActorRelativeDistanceTouchExtractorSession : ITouchExtractorSession<float>
        {
            private readonly Axes _axes;

            private Vector3 _actorPositionReference;

            public ActorRelativeDistanceTouchExtractorSession(Axes axes)
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
        
        private class ActorToSourceDistanceTouchExtractorSession : ITouchExtractorSession<float>
        {
            private readonly bool _normalized;
            private readonly Axes _axes;

            private float _referenceDistance;

            public ActorToSourceDistanceTouchExtractorSession(Axes axes, bool normalized)
            {
                _normalized = normalized;
                _axes = axes;
            }

            private bool Common(TouchEvent touchEvent, out float value)
            {
                float distance = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes).magnitude;
                value = _normalized ? distance / _referenceDistance : distance;
                // Debug.Log($"{this} {nameof(Common)} {nameof(distance)} {distance} {nameof(_referenceDistance)} {_referenceDistance} {nameof(value)} {value}");
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _referenceDistance = math.max(touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes).magnitude, 1e-3f);
                // Debug.Log($"{this} {nameof(Setup)} {nameof(_referenceDistance)} {_referenceDistance}");
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }

        private class TwistTouchExtractorSession : ITouchExtractorSession<float>
        {
            private Quaternion _referenceRotation;

            private bool Common(TouchEvent touchEvent, out float value)
            {
                Quaternion rotation = touchEvent.TouchData.Actor.transform.rotation;
                value = Quaternion.Angle(_referenceRotation, rotation) / 180f;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out float value)
            {
                _referenceRotation = touchEvent.TouchData.Actor.transform.rotation;
                return Common(touchEvent, out value);
            }

            public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
    }

}