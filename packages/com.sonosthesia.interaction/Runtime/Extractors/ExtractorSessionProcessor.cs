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

        protected abstract TValue Process(TEvent touchEvent, TValue value);

        public bool Setup(TEvent touchEvent, out TValue value)
        {
            if (_session.Setup(touchEvent, out value))
            {
                value = Process(touchEvent, value);
                return true;
            }
            return false;
        }
        
        public bool Update(TEvent touchEvent, out TValue value)
        {
            if (_session.Update(touchEvent, out value))
            {
                value = Process(touchEvent, value);
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

        protected override float Process(TEvent touchEvent, float value) => _settings.Remap(value);
    }
    
    public class FloatCurveSession<TEvent> : ExtractorSessionProcessor<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly AnimationCurve _curve;
        
        public FloatCurveSession(IExtractorSession<TEvent, float> session, AnimationCurve curve) : base(session)
        {
            _curve = curve;
        }

        protected override float Process(TEvent touchEvent, float value) => _curve.Evaluate(value);
    }

    public class FloatClampSession<TEvent> : ExtractorSessionProcessor<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly FloatRange _range;
        
        public FloatClampSession(IExtractorSession<TEvent, float> session, FloatRange range) : base(session)
        {
            _range = range;
        }

        protected override float Process(TEvent touchEvent, float value) => _range.Clamp(value);
    }

    public class FloatStaticSession<TEvent> : IExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly float _staticValue;
        
        public FloatStaticSession(float staticValue)
        {
            _staticValue = staticValue;
        }
        
        private bool Common(TEvent touchEvent, out float value)
        {
            value = _staticValue;
            return true;
        }

        public bool Setup(TEvent touchEvent, out float value) => Common(touchEvent, out value);

        public bool Update(TEvent touchEvent, out float value) => Common(touchEvent, out value);
    }
}