using System;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Generator
{
    [Serializable] 
    public class EaseEnvelopePhase
    {
        public static EaseEnvelopePhase Linear(float duration)
        {
            return new EaseEnvelopePhase(EaseType.linear, duration);
        }

        public EaseEnvelopePhase() { }

        public EaseEnvelopePhase(EaseType easeType, float duration)
        {
            _easeType = easeType;
            _duration = duration;
        }
        
        [SerializeField] private EaseType _easeType;
        public EaseType EaseType => _easeType;

        [SerializeField] private float _duration = 1f;
        public float Duration => _duration;

        public float Evaluate(float time)
        {
            if (_duration < 1e-3)
            {
                return 1f;
            }
            return _easeType.Evaluate(time / _duration);
        }
    }
}