using System;

namespace Sonosthesia.Extractor
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
            value = default;
            if (_session.Setup(e, out TValue reference))
            {
                _reference = reference;
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