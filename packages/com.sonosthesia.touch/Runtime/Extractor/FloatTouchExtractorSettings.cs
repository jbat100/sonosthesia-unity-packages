using System;
using Sonosthesia.Dynamic;
using Sonosthesia.Utils;
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
            Distance
        }

        public enum DynamicType
        {
            Actor,
            Source,
            Relative
        }

        public enum DistanceType
        {
            Actor,
            Relative
        }

        public enum PostProcessingType
        {
            None,
            Remap,
            Curve
        }
        
        [SerializeField] private ExtractorType _extractorType = ExtractorType.Static;

        [SerializeField] private TouchExtractor<float> _extractor;
        
        [SerializeField] private float _staticValue;

        [SerializeField] private DynamicType _dynamicType = DynamicType.Actor;
        [SerializeField] private TransformDynamics.Domain _dynamicsDomain = TransformDynamics.Domain.Position;
        [SerializeField] private TransformDynamics.Order _dynamicsOrder = TransformDynamics.Order.Velocity;

        [SerializeField] private DistanceType _distanceType = DistanceType.Actor;
        [SerializeField] private Axes _distanceAxes = Axes.X | Axes.Y | Axes.Z;

        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;

        public ITouchExtractorSession<float> MakeSession()
        {
            ITouchExtractorSession<float> session = _extractorType switch
            {
                ExtractorType.Static => new StaticFloatTouchExtractorSession(_staticValue),
                ExtractorType.Dynamic => new DynamicFloatTouchExtractorSession(_dynamicType, _dynamicsDomain, _dynamicsOrder),
                ExtractorType.Distance => new DistanceFloatTouchExtractorSession(_distanceType, _distanceAxes),
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
    }

    public class RemapTouchExtractorSession : TouchExtractorSessionProcessor<float, RemapSettings>
    {
        public RemapTouchExtractorSession(ITouchExtractorSession<float> session, RemapSettings processor) : base(session, processor)
        {
        }

        protected override float Process(RemapSettings processor, float value)
        {
            return processor.Remap(value);
        }
    }
    
    public class CurveTouchExtractorSession : TouchExtractorSessionProcessor<float, AnimationCurve>
    {
        public CurveTouchExtractorSession(ITouchExtractorSession<float> session, AnimationCurve processor) : base(session, processor)
        {
        }

        protected override float Process(AnimationCurve processor, float value)
        {
            return processor.Evaluate(value);
        }
    }

    public class StaticFloatTouchExtractorSession : ITouchExtractorSession<float>
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

    public class DynamicFloatTouchExtractorSession : ITouchExtractorSession<float>
    {
        private readonly FloatTouchExtractorSettings.DynamicType _type;
        private readonly TransformDynamics.Domain _domain;
        private readonly TransformDynamics.Order _order;

        private TransformDynamicsMonitor _sourceMonitor;
        private TransformDynamicsMonitor _actorMonitor;
        
        public DynamicFloatTouchExtractorSession(FloatTouchExtractorSettings.DynamicType type, TransformDynamics.Domain domain, TransformDynamics.Order order)
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
                FloatTouchExtractorSettings.DynamicType.Actor => actor.magnitude,
                FloatTouchExtractorSettings.DynamicType.Source => source.magnitude,
                FloatTouchExtractorSettings.DynamicType.Relative => (actor - source).magnitude,
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
    
    public class DistanceFloatTouchExtractorSession : ITouchExtractorSession<float>
    {
        private readonly FloatTouchExtractorSettings.DistanceType _type;
        private readonly Axes _axes;

        private Vector3 _actorPositionReference;

        public DistanceFloatTouchExtractorSession(FloatTouchExtractorSettings.DistanceType type, Axes axes)
        {
            _type = type;
            _axes = axes;
        }

        private bool Common(TouchEvent touchEvent, out float value)
        {
            Vector3 actorPosition = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes);
            value = _type switch
            {
                FloatTouchExtractorSettings.DistanceType.Actor => (_actorPositionReference - actorPosition).magnitude,
                FloatTouchExtractorSettings.DistanceType.Relative => actorPosition.magnitude,
                _ => throw new ArgumentOutOfRangeException()
            };
            return true;
        }
        
        public bool Setup(TouchEvent touchEvent, out float value)
        {
            _actorPositionReference = touchEvent.ActorPositionInSourceSpace().FilterAxes(_axes);
            return Common(touchEvent, out value);
        }

        public bool Update(TouchEvent touchEvent, out float value) => Common(touchEvent, out value);
    }
}