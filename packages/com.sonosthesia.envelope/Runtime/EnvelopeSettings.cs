using System;
using UnityEngine;

namespace Sonosthesia.Envelope
{
    public enum EnvelopeType
    {
        Custom,
        Phased,
        Curve
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

            if (settings.EnvelopeType == EnvelopeType.Custom)
            {
                return settings.EnvelopeFactory ? settings.EnvelopeFactory.Build() : null;
            }

            if (settings.EnvelopeType == EnvelopeType.Curve)
            {
                return new AnimationCurveEnvelope(settings.AnimationCurve);
            }

            if (settings.EnvelopeType == EnvelopeType.Phased)
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

            return null;
        }
    }
    
    
    [Serializable]
    public class EnvelopeSettings
    {
        [SerializeField] private EnvelopeType _envelopeType;
        public EnvelopeType EnvelopeType => _envelopeType;

        [SerializeField] private AbstractEnvelopeFactory _envelopeFactory;
        public AbstractEnvelopeFactory EnvelopeFactory => _envelopeFactory;
        
        [SerializeField] private AnimationCurve _animationCurve;
        public AnimationCurve AnimationCurve => _animationCurve;
        
        [SerializeField] private PhasedEnvelopeType _phasedType;
        public PhasedEnvelopeType PhasedType => _phasedType;
        
        [SerializeField] private EnvelopePhase _attack = EnvelopePhase.Linear(1f);
        public EnvelopePhase Attack => _attack;
        
        [SerializeField] private EnvelopePhase _decay = EnvelopePhase.Linear(0.5f);
        public EnvelopePhase Decay => _decay;
        
        [SerializeField] private EnvelopePhase _release = EnvelopePhase.Linear(1f);
        public EnvelopePhase Release => _release;
        
        [SerializeField] private float _hold;
        public float Hold => _hold;
        
        [SerializeField] private float _sustain = 0.5f;
        public float Sustain => _sustain;
    }
}