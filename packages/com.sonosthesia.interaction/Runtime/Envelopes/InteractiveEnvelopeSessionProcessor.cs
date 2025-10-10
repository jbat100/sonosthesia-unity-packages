using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class InteractiveEnvelopeSessionProcessor<TEvent> : IInteractiveEnvelopeSession<TEvent>
    {
        private readonly IInteractiveEnvelopeSession<TEvent> _session;

        public InteractiveEnvelopeSessionProcessor(IInteractiveEnvelopeSession<TEvent> session)
        {
            _session = session;
        }

        public void Start(TEvent e) => _session.Start(e);

        public void Update(TEvent e) => _session.Update(e);

        public void End(TEvent e, out float release) => _session.End(e, out release);

        public float Update() => Process(_session.Update());

        protected abstract float Process(float value);
    }

    // tracks settings, used for testing for build use StaticTouchEnvelopeSessionOneEuroFilter
    public class InteractiveEnvelopeSessionOneEuroFilter<TEvent>  : InteractiveEnvelopeSessionProcessor<TEvent>
    {
        private readonly OneEuroFilter1 _filter;
        private readonly OneEuroFilterSettings _settings;
        private readonly float _startTime;

        public InteractiveEnvelopeSessionOneEuroFilter(IInteractiveEnvelopeSession<TEvent> session, OneEuroFilterSettings settings) : base(session)
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
    
    public class StaticInteractiveEnvelopeSessionOneEuroFilter<TEvent>  : InteractiveEnvelopeSessionProcessor<TEvent>
    {
        private readonly OneEuroFilter1 _filter;
        private readonly float _startTime;

        public StaticInteractiveEnvelopeSessionOneEuroFilter(IInteractiveEnvelopeSession<TEvent> session, OneEuroFilterSettings settings) : base(session)
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