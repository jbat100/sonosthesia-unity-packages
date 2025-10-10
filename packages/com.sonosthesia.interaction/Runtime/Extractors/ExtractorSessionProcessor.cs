using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class ExtractorSessionProcessor<TEvent, TValue> : IExtractorSession<TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        private readonly IExtractorSession<TEvent, TValue> _session;
        
        public ExtractorSessionProcessor(IExtractorSession<TEvent, TValue> session)
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

    public class FloatRemapSession<TEvent> : ExtractorSessionProcessor<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly RemapSettings _settings;
        
        public FloatRemapSession(IExtractorSession<TEvent, float> session, RemapSettings settings) : base(session)
        {
            _settings = settings;
        }

        protected override float Process(TEvent e, float value) => _settings.Remap(value);
    }
    
    public class FloatCurveSession<TEvent> : ExtractorSessionProcessor<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly AnimationCurve _curve;
        
        public FloatCurveSession(IExtractorSession<TEvent, float> session, AnimationCurve curve) : base(session)
        {
            _curve = curve;
        }

        protected override float Process(TEvent e, float value) => _curve.Evaluate(value);
    }

    public class FloatClampSession<TEvent> : ExtractorSessionProcessor<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly FloatRange _range;
        
        public FloatClampSession(IExtractorSession<TEvent, float> session, FloatRange range) : base(session)
        {
            _range = range;
        }

        protected override float Process(TEvent e, float value) => _range.Clamp(value);
    }

    public class FloatStaticSession<TEvent> : IExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly float _staticValue;
        
        public FloatStaticSession(float staticValue)
        {
            _staticValue = staticValue;
        }
        
        private bool Common(TEvent e, out float value)
        {
            value = _staticValue;
            return true;
        }

        public bool Setup(TEvent e, out float value) => Common(e, out value);

        public bool Update(TEvent e, out float value) => Common(e, out value);
    }

    public class InitialSession<TEvent, TValue> : IExtractorSession<TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        private readonly IExtractorSession<TEvent, TValue> _session;
        private TValue? _initial;
        
        public InitialSession(IExtractorSession<TEvent, TValue> session)
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
    
    public abstract class RelativeSession<TEvent, TValue> : IExtractorSession<TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        private readonly IExtractorSession<TEvent, TValue> _session;
        private TValue? _reference;

        protected RelativeSession(IExtractorSession<TEvent, TValue> session)
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

    public class FloatRelativeSession<TEvent> : RelativeSession<TEvent, float> where TEvent : IInteractionEvent
    {
        public FloatRelativeSession(IExtractorSession<TEvent, float> session) : base(session)
        {
        }

        protected override float Relative(float value, float reference) => value - reference;
    }
}