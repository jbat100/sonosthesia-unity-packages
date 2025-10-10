using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class ExtractorSessionProcessor<TEvent, TValue> : IDynamicExtractorSession<TEvent, TValue> 
        where TValue : struct
    {
        private readonly IDynamicExtractorSession<TEvent, TValue> _session;
        
        public ExtractorSessionProcessor(IDynamicExtractorSession<TEvent, TValue> session)
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

    public class FloatRemapSession<TEvent> : ExtractorSessionProcessor<TEvent, float>
    {
        private readonly RemapSettings _settings;
        
        public FloatRemapSession(IDynamicExtractorSession<TEvent, float> session, RemapSettings settings) : base(session)
        {
            _settings = settings;
        }

        protected override float Process(TEvent e, float value) => _settings.Remap(value);
    }
    
    public class FloatCurveSession<TEvent> : ExtractorSessionProcessor<TEvent, float>
    {
        private readonly AnimationCurve _curve;
        
        public FloatCurveSession(IDynamicExtractorSession<TEvent, float> session, AnimationCurve curve) : base(session)
        {
            _curve = curve;
        }

        protected override float Process(TEvent e, float value) => _curve.Evaluate(value);
    }

    public class FloatClampSession<TEvent> : ExtractorSessionProcessor<TEvent, float>
    {
        private readonly FloatRange _range;
        
        public FloatClampSession(IDynamicExtractorSession<TEvent, float> session, FloatRange range) : base(session)
        {
            _range = range;
        }

        protected override float Process(TEvent e, float value) => _range.Clamp(value);
    }

    public class FloatConstantSession<TEvent> : StatelessExtractorSession<TEvent, float>
    {
        private readonly float _staticValue;
        
        public FloatConstantSession(float staticValue)
        {
            _staticValue = staticValue;
        }

        protected override bool Extract(TEvent interactionEvent, out float value)
        {
            value = _staticValue;
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
}