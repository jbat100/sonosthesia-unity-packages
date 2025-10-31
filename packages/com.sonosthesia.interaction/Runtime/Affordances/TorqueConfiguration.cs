using System;
using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface ITorqueConfiguration<in TEvent>
    {
        bool Track { get; }
        ForceMode ForceMode { get; }
        bool Relative { get; }
        IDynamicExtractor<TEvent, Vector3> Torque { get; }
        IInteractiveEnvelopeSettings<TEvent> Intensity { get; }
    }

    [Serializable]
    public class TorqueConfiguration<TEvent, TEnvelope, TExtractor> : ScriptableObject
        where TEvent : struct
        where TEnvelope : IInteractiveEnvelopeSettings<TEvent>
        where TExtractor : IDynamicExtractor<TEvent, Vector3>
    {
        [SerializeField] private bool _track;
        public bool Track => _track;

        [SerializeField] private ForceMode _forceMode;
        public ForceMode ForceMode => _forceMode;

        [SerializeField] private bool _relative;
        public bool Relative => _relative;
        
        [SerializeField] private TExtractor _torque;
        public IDynamicExtractor<TEvent, Vector3> Torque => _torque;

        [SerializeField] private TEnvelope _intensity;
        public IInteractiveEnvelopeSettings<TEvent> Intensity => _intensity;
    }
}