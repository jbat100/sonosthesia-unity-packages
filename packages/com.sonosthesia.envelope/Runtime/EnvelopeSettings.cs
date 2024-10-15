using System;
using UnityEngine;

namespace Sonosthesia.Envelope
{
    public enum EnvelopeType
    {
        Custom,
        Phased,
        Curve,
        Constant
    }
    
    public enum PhasedEnvelopeType
    {
        AHR,
        ADSR,
        ADS,
        SR
    }

    public static class EnvelopeUtils
    {
        public static IEnvelope Build(this EnvelopeSettings settings)
        {
            if (settings == null)
            {
                return null;
            }

            IEnvelope PhasedEnvelope()
            {
                return settings.PhasedType switch
                {
                    PhasedEnvelopeType.AHR => new AHREnvelope(settings.Attack, settings.Hold, settings.Release),
                    PhasedEnvelopeType.ADSR => new ADSREnvelope(settings.Attack, settings.Decay, settings.Sustain, settings.Hold, settings.Release),
                    PhasedEnvelopeType.ADS => new ADSEnvelope(settings.Attack, settings.Decay, settings.Sustain),
                    PhasedEnvelopeType.SR => new SREnvelope(settings.Sustain, settings.Release),
                    _ => null
                }; 
            }

            return settings.EnvelopeType switch
            {
                EnvelopeType.Custom => settings.EnvelopeFactory ? settings.EnvelopeFactory.Build() : null,
                EnvelopeType.Phased => PhasedEnvelope(),
                EnvelopeType.Curve => new AnimationCurveEnvelope(settings.AnimationCurve),
                EnvelopeType.Constant => new ConstantEnvelope(settings.ConstantValue, settings.ConstantDuration),
                _ => null
            };
        }
    }

    [Serializable]
    public class EnvelopeSettings
    {
        public static EnvelopeSettings ADSR(EnvelopePhase attack, EnvelopePhase decay, float sustain, float hold, EnvelopePhase release)
        {
            return new EnvelopeSettings
            {
                _envelopeType = EnvelopeType.Phased,
                _phasedType = PhasedEnvelopeType.ADSR,
                _attack = attack,
                _decay = decay,
                _sustain = sustain,
                _hold = hold,
                _release = release
            };
        }
        
        public static EnvelopeSettings AHR(EnvelopePhase attack, float hold, EnvelopePhase release)
        {
            return new EnvelopeSettings
            {
                _envelopeType = EnvelopeType.Phased,
                _phasedType = PhasedEnvelopeType.AHR,
                _attack = attack,
                _hold = hold,
                _release = release
            };
        }
        
        public static EnvelopeSettings ADS(EnvelopePhase attack, EnvelopePhase decay, float sustain)
        {
            return new EnvelopeSettings
            {
                _envelopeType = EnvelopeType.Phased,
                _phasedType = PhasedEnvelopeType.ADS,
                _attack = attack,
                _decay = decay,
                _sustain = sustain
            };
        }
        
        public static EnvelopeSettings SR(float sustain, EnvelopePhase release)
        {
            return new EnvelopeSettings
            {
                _envelopeType = EnvelopeType.Phased,
                _phasedType = PhasedEnvelopeType.SR,
                _sustain = sustain,
                _release = release
            };
        }

        public static EnvelopeSettings SR(EnvelopePhase release) => SR(1f, release);

        [SerializeField] private EnvelopeType _envelopeType = EnvelopeType.Phased;
        public EnvelopeType EnvelopeType => _envelopeType;

        [SerializeField] private float _constantValue = 1f;
        public float ConstantValue => _constantValue;
        
        [SerializeField] private float _constantDuration = 1f;
        public float ConstantDuration => _constantDuration;
        
        [SerializeField] private AbstractEnvelopeFactory _envelopeFactory;
        public AbstractEnvelopeFactory EnvelopeFactory => _envelopeFactory;
        
        [SerializeField] private AnimationCurve _animationCurve;
        public AnimationCurve AnimationCurve => _animationCurve;
        
        [SerializeField] private PhasedEnvelopeType _phasedType = PhasedEnvelopeType.ADSR;
        public PhasedEnvelopeType PhasedType => _phasedType;
        
        [SerializeField] private EnvelopePhase _attack = EnvelopePhase.Linear(1f);
        public EnvelopePhase Attack => _attack;
        
        [SerializeField] private EnvelopePhase _decay = EnvelopePhase.Linear(0.5f);
        public EnvelopePhase Decay => _decay;
        
        [SerializeField] private EnvelopePhase _release = EnvelopePhase.Linear(1f);
        public EnvelopePhase Release => _release;
        
        [SerializeField] private float _hold = 1f;
        public float Hold => _hold;
        
        [SerializeField] private float _sustain = 0.5f;
        public float Sustain => _sustain;
    }
}