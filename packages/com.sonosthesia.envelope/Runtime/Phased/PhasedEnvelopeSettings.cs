using System;
using UnityEngine;

namespace Sonosthesia.Envelope
{
    [Serializable]
    public class PhasedEnvelopeSettings
    {
        [SerializeField] private PhasedEnvelopeType _envelopeType;
        [SerializeField] private EnvelopePhase _attack = EnvelopePhase.Linear(1f);
        [SerializeField] private EnvelopePhase _decay = EnvelopePhase.Linear(0.5f);
        [SerializeField] private EnvelopePhase _release = EnvelopePhase.Linear(1f);
        [SerializeField] private float _hold;
        [SerializeField] private float _sustain = 0.5f;
        
        public PhasedEnvelopeType Type => _envelopeType;
        public EnvelopePhase Attack => _attack;
        public EnvelopePhase Decay => _decay;
        public EnvelopePhase Release => _release;
        public float Hold => _hold;
        public float Sustain => _sustain;
    }
}