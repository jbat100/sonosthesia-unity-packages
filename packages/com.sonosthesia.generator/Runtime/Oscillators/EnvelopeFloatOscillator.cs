using System;
using Sonosthesia.Envelope;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Generator
{
    public class EnvelopeFloatOscillator : FloatOscillator
    {
        [SerializeField] private EnvelopeBuilder _envelopeBuilder;

        private IEnvelope _envelope;
        private IDisposable _subscription;

        private void RefreshSubscription()
        {
            _subscription?.Dispose();
            if (_envelopeBuilder)
            {
                _subscription = _envelopeBuilder.ChangeObservable.StartWith(Unit.Default).Subscribe(_ => RefreshEnvelope());
            }
        }
        
        private void RefreshEnvelope()
        {
            _envelope = _envelopeBuilder ? _envelopeBuilder.Build() : null;
        }

        protected virtual void OnValidate() => RefreshSubscription();

        protected virtual void Start() => RefreshSubscription();

        protected virtual void OnDestroy() => _subscription?.Dispose();

        protected override float Duration => _envelope?.Duration ?? 0f;

        protected override float EvaluateIteration(float time) => _envelope?.Evaluate(time) ?? 0f;
    }
}