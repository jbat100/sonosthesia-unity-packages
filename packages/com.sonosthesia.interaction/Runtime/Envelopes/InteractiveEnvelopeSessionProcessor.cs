using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class InteractiveEnvelopeSessionProcessor<TEvent> : IInteractiveEnvelopeSession<TEvent>
    {
        private readonly IInteractiveEnvelopeSession<TEvent> _session;

        protected InteractiveEnvelopeSessionProcessor(IInteractiveEnvelopeSession<TEvent> session)
        {
            _session = session;
        }

        public void Start(TEvent e) => _session.Start(e);
        public void Update(TEvent e) => _session.Update(e);
        public void End(TEvent e, out float release) => _session.End(e, out release);

        public float Evaluate() => Process(_session.Evaluate());

        protected abstract float Process(float value);
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

        protected override float Process(float value) => _filter.Step(Time.time - _startTime, value);
    }
}