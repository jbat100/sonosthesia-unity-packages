using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class ExtractorSessionProcessor<TEvent, TValue> : IDynamicExtractorSession<TEvent, TValue> 
        where TValue : struct
    {
        private readonly IDynamicExtractorSession<TEvent, TValue> _session;

        protected ExtractorSessionProcessor(IDynamicExtractorSession<TEvent, TValue> session)
        {
            _session = session;
        }

        protected abstract TValue Process(TEvent e, TValue value);

        public bool Setup(TEvent e, out TValue value)
        {
            if (_session.Setup(e, out value))
            {
                value = Process(e, value);
                return true;
            }
            return false;
        }
        
        public bool Update(TEvent e, out TValue value)
        {
            if (_session.Update(e, out value))
            {
                value = Process(e, value);
                return true;
            }
            return false;
        }
    }

    public class FuncExtractionSessionProcessor<TEvent, TValue> : ExtractorSessionProcessor<TEvent, TValue> where TValue : struct
    {
        private readonly Func<TValue, TValue> _process;

        public FuncExtractionSessionProcessor(IDynamicExtractorSession<TEvent, TValue> session, Func<TValue, TValue> process) : base(session)
        {
            _process = process;
        }

        protected override TValue Process(TEvent e, TValue value) => _process(value);
    }
 
    public class ConstantExtractorSession<TEvent, TValue> : StatelessExtractorSession<TEvent, TValue> where TValue : struct
    {
        private readonly TValue _constantValue;
        
        public ConstantExtractorSession(TValue constantValue)
        {
            _constantValue = constantValue;
        }

        protected override bool Extract(TEvent e, out TValue value)
        {
            value = _constantValue;
            return true;
        }
    }

    public class InitialSession<TEvent, TValue> : IDynamicExtractorSession<TEvent, TValue> where TValue : struct
    {
        private readonly IDynamicExtractorSession<TEvent, TValue> _session;
        private TValue? _initial;
        
        public InitialSession(IDynamicExtractorSession<TEvent, TValue> session)
        {
            _session = session;
        }
        
        public bool Setup(TEvent e, out TValue value)
        {
            if (_session.Setup(e, out value))
            {
                _initial = value;
                return true;
            }
            return false;
        }
        
        public bool Update(TEvent e, out TValue value)
        {
            if (_initial.HasValue)
            {
                value = _initial.Value;
                return true;
            }
            value = default;
            return false;
        }
    }
    
    public abstract class RelativeSession<TEvent, TValue> : IDynamicExtractorSession<TEvent, TValue> where TValue : struct
    {
        private readonly IDynamicExtractorSession<TEvent, TValue> _session;
        private TValue? _reference;

        protected RelativeSession(IDynamicExtractorSession<TEvent, TValue> session)
        {
            _session = session;
        }
        
        public bool Setup(TEvent e, out TValue value)
        {
            if (_session.Setup(e, out value))
            {
                _reference = value;
                return true;
            }
            return false;
        }
        
        public bool Update(TEvent e, out TValue value)
        {
            if (_reference.HasValue && _session.Update(e, out TValue raw))
            {
                value = Relative(raw, _reference.Value);
                return true;
            }

            value = default;
            return false;
        }

        protected abstract TValue Relative(TValue value, TValue reference);
    }

    public class FloatRelativeSession<TEvent> : RelativeSession<TEvent, float>
    {
        public FloatRelativeSession(IDynamicExtractorSession<TEvent, float> session) : base(session)
        {
        }

        protected override float Relative(float value, float reference) => value - reference;
    }

    public class FloatNormalizedSession<TEvent> : IDynamicExtractorSession<TEvent, float>
    {
        private readonly IDynamicExtractorSession<TEvent, float> _session;
        private float? _reference;
        
        public FloatNormalizedSession(IDynamicExtractorSession<TEvent, float> session)
        {
            _session = session;
        }
        
        public bool Setup(TEvent e, out float value)
        {
            if (_session.Setup(e, out float raw))
            {
                _reference = raw;
                value = 1f;
                return true;
            }
            value = 0;
            return false;
        }
        
        public bool Update(TEvent e, out float value)
        {
            if (_reference.HasValue && _reference.Value != 0 && _session.Update(e, out float raw))
            {
                value = raw / _reference.Value;
                return true;
            }

            value = 0;
            return false;
        }
    }
    
    public class VelocityFloatExtractorSession<TEvent> : StatelessExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly VelocityExtractionType _type;
            
        public VelocityFloatExtractorSession(VelocityExtractionType type)
        {
            _type = type;
        }

        protected override bool Extract(TEvent e, out float value) => e.ExtractVelocity(_type, out value);
    }
    
    public class DirectionVectorExtractorSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        private readonly ExtractionSpace _space;
        private readonly Vector3 _direction;
            
        public DirectionVectorExtractorSession(ExtractionSpace space, Vector3 direction)
        {
            _space = space;
            _direction = direction;
        }

        protected override bool Extract(TEvent e, out Vector3 value) => e.TransformPoint(_space, _direction, out value);
    }

    public class VelocityVectorExtractorSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        private readonly VelocityExtractionType _velocityType;
            
        public VelocityVectorExtractorSession(VelocityExtractionType velocityType)
        {
            _velocityType = velocityType;
        }
            
        protected override bool Extract(TEvent e, out Vector3 value) => e.ExtractVelocity(_velocityType, out value);
    }
    
    public class VectorRelativeSession<TEvent> : RelativeSession<TEvent, Vector3>
    {
        public VectorRelativeSession(IDynamicExtractorSession<TEvent, Vector3> session) : base(session)
        {
        }

        protected override Vector3 Relative(Vector3 value, Vector3 reference) => value - reference;
    }

    public class RelativePositionVectorExtractionSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        protected override bool Extract(TEvent e, out Vector3 value) => e.ExtractRelativePosition(out value);
    }

    public class AxisVectorExtractionSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        protected override bool Extract(TEvent e, out Vector3 value) => e.ExtractAxis(out value);
    }
    
    public class ActorToSourceDistanceSession<TEvent> : StatelessExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly Axes _axes;
            
        public ActorToSourceDistanceSession(Axes axes)
        {
            _axes = axes;
        }

        protected override bool Extract(TEvent e, out float value) => e.ActorToSourceDistance(_axes, out value);
    }

    public class HeightFloatExtractorSession<TEvent> : StatelessExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        protected override bool Extract(TEvent e, out float value)
        {
            value = e.Actor.Transform.position.y;
            return true;
        }
    }

    public class TwistFloatExtractorSession<TEvent> : IDynamicExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private Quaternion _referenceRotation;

        private bool Common(TEvent e, out float value)
        {
            Quaternion rotation = e.Actor.Transform.rotation;
            value = Quaternion.Angle(_referenceRotation, rotation) / 180f;
            return true;
        }
            
        public bool Setup(TEvent e, out float value)
        {
            _referenceRotation = e.Actor.Transform.rotation;
            return Common(e, out value);
        }

        public bool Update(TEvent e, out float value) => Common(e, out value);
    }

}