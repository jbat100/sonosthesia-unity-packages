using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public abstract class TouchEnvelopeSessionProcessor : ITouchEnvelopeSession
    {
        private readonly ITouchEnvelopeSession _session;

        public TouchEnvelopeSessionProcessor(ITouchEnvelopeSession session)
        {
            _session = session;
        }

        public void StartTouch(TouchEvent e) => _session.StartTouch(e);

        public void UpdateTouch(TouchEvent e) => _session.UpdateTouch(e);

        public void EndTouch(TouchEvent e, out float release) => _session.EndTouch(e, out release);

        public float Update() => Process(_session.Update());

        protected abstract float Process(float value);
    }

    // tracks settings, used for testing for build use StaticTouchEnvelopeSessionOneEuroFilter
    public class TouchEnvelopeSessionOneEuroFilter : TouchEnvelopeSessionProcessor
    {
        private readonly OneEuroFilter1 _filter;
        private readonly OneEuroFilterSettings _settings;
        private readonly float _startTime;

        public TouchEnvelopeSessionOneEuroFilter(ITouchEnvelopeSession session, OneEuroFilterSettings settings) : base(session)
        {
            _startTime = Time.time;
            _filter = new OneEuroFilter1();
            _settings = settings;
            _settings.ApplyTo(_filter);
        }

        protected override float Process(float value)
        {
            _settings.ApplyTo(_filter);
            return _filter.Step(Time.time - _startTime, value);
        }
    }
    
    public class StaticTouchEnvelopeSessionOneEuroFilter : TouchEnvelopeSessionProcessor
    {
        private readonly OneEuroFilter1 _filter;
        private readonly float _startTime;

        public StaticTouchEnvelopeSessionOneEuroFilter(ITouchEnvelopeSession session, OneEuroFilterSettings settings) : base(session)
        {
            _startTime = Time.time;
            _filter = new OneEuroFilter1();
            settings.ApplyTo(_filter);
        }

        protected override float Process(float value)
        {
            return _filter.Step(Time.time - _startTime, value);
        }
    }
}