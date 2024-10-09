using System;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Envelope
{
    [Serializable] 
    public struct EnvelopePhase
    {
        public static EnvelopePhase Linear(float duration)
        {
            return new EnvelopePhase(EaseType.linear, duration);
        }

        public EnvelopePhase(EaseType easeType, float duration)
        {
            _easeType = easeType; 
            _duration = duration;
        }
        
        [SerializeField] private EaseType _easeType;
        public EaseType EaseType => _easeType;

        [SerializeField] private float _duration;
        public float Duration => _duration;

        public float Evaluate(float time) => _duration < 1e-3 ? 1f : _easeType.Evaluate(time / _duration);
    }
    
    public class AHREnvelope : IEnvelope
    {
        private readonly float _hold;
        
        // not readonly to avoid defensive copy:
        // possibly impure struct method called on readonly variable: struct value always copied before invocation
        
        private EnvelopePhase _attack;
        private EnvelopePhase _release;

        public AHREnvelope(EnvelopePhase attack, float hold, EnvelopePhase release)
        {
            _attack = attack;
            _hold = hold;
            _release = release;
        }
        
        public float Duration => _attack.Duration + _hold + _release.Duration;

        public float InitialValue => 0f;
        
        public float FinalValue => 0f;

        public float Evaluate(float t)
        {
            const float attackStart = 0;
            float holdStart = _attack.Duration;
            float releaseStart = _attack.Duration + _hold;
            float end = releaseStart + _release.Duration;
            
            // attack phase
            if (t >= attackStart && t < holdStart)
            {
                return _attack.Evaluate(t);
            }
            // hold phase
            if (t >= holdStart && t < releaseStart)
            {
                return 1f;
            }
            // release phase
            if (t >= releaseStart && t < end)
            {
                if (_release.Duration < 1e-3)
                {
                    return 0f;
                }
                return 1f - _release.Evaluate(t - releaseStart);
            }
            return 0f;
        }
    }
    
    public class ADSREnvelope : IEnvelope
    {
        private readonly float _hold;
        private readonly float _sustain;
        
        // not readonly to avoid defensive copy:
        // possibly impure struct method called on readonly variable: struct value always copied before invocation
        
        private EnvelopePhase _attack;
        private EnvelopePhase _decay;
        private EnvelopePhase _release;

        public ADSREnvelope(EnvelopePhase attack, EnvelopePhase decay, float sustain, float hold, EnvelopePhase release)
        {
            _attack = attack;
            _decay = decay;
            _hold = hold;
            _sustain = sustain;
            _release = release;
        }
        
        public float Duration => _attack.Duration + _decay.Duration + _hold + _release.Duration;

        public float InitialValue => 0f;
        
        public float FinalValue => 0f;
        
        public float Evaluate(float t)
        {
            const float attackStart = 0f;
            float decayStart = _attack.Duration;
            float holdStart = decayStart + _decay.Duration;
            float releaseStart = holdStart + _hold;
            float end = releaseStart + _release.Duration;
            
            // attack phase
            if (t >= attackStart && t < decayStart)
            {
                return _attack.Evaluate(t);
            }
            // decay phase
            if (t >= decayStart && t < holdStart)
            {
                return 1 - _decay.Evaluate(t - decayStart) * (1 - _sustain);
            }
            // hold phase
            if (t >= holdStart && t < releaseStart)
            {
                return _sustain;
            }
            // release phase
            if (t >= releaseStart && t < end)
            {
                return (1f - _release.Evaluate(t - releaseStart)) * _sustain;
            }
            return 0f;
        }
    }

    public class ADSEnvelope : IEnvelope
    {
        private readonly float _sustain;
        
        // not readonly to avoid defensive copy:
        // possibly impure struct method called on readonly variable: struct value always copied before invocation
        
        private EnvelopePhase _attack;
        private EnvelopePhase _decay;

        public ADSEnvelope(EnvelopePhase attack, EnvelopePhase decay, float sustain)
        {
            _attack = attack;
            _decay = decay;
            _sustain = sustain;
        }

        public float Duration => _attack.Duration + _decay.Duration;

        public float InitialValue => 0f;
        
        public float FinalValue => _sustain;

        public float Evaluate(float t)
        {
            const float attackStart = 0f;
            float decayStart = _attack.Duration;
            float end = decayStart + _decay.Duration;
            
            // attack phase
            if (t >= attackStart && t < decayStart)
            {
                return _attack.Evaluate(t);
            }
            // decay phase
            if (t >= decayStart && t < end)
            {
                return 1f - _decay.Evaluate(t - decayStart) * (1f - _sustain);
            }
            return _sustain;
        }
    }

    public class SREnvelope : IEnvelope
    {
        private readonly float _sustain;
        
        // not readonly to avoid defensive copy:
        // possibly impure struct method called on readonly variable: struct value always copied before invocation
        
        private EnvelopePhase _release;

        public SREnvelope(float sustain, EnvelopePhase release)
        {
            _sustain = sustain;
            _release = release;
        }
        
        public float Duration => _release.Duration;

        public float InitialValue => _sustain;
        
        public float FinalValue => 0f;

        public float Evaluate(float t)
        {
            if (_release.Duration < 1e-3)
            {
                return 0f;
            }
            if (t < _release.Duration)
            {
                return (1f - _release.Evaluate(t)) * _sustain;
            }
            return 0;
        }
    }
}