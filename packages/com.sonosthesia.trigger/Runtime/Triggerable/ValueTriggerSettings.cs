using System;
using Sonosthesia.Envelope;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [Serializable]
    public class ValueTriggerSettings<T> where T : struct
    {
        [SerializeField] private Selector<T> _valueSelector;
        [SerializeField] private Selector<T> _timeSelector;
        [SerializeField] private EnvelopeFactory _envelopeFactory;

        public void Trigger(Triggerable triggerable, T value)
        {
            IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            triggerable.Trigger(envelope, valueScale, timeScale);
        }
    }

    [Serializable]
    public class ValueStartTriggerSettings<T> where T : struct
    {
        [SerializeField] private Selector<T> _valueSelector;
        [SerializeField] private Selector<T> _timeSelector;
        [SerializeField] private EnvelopeFactory _envelopeFactory;

        public Guid StartTrigger(TrackedTriggerable triggerable, T value)
        {
            IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            return triggerable.StartTrigger(envelope, valueScale, timeScale);
        }
    }
    
    [Serializable]
    public class ValueEndTriggerSettings<T> where T : struct
    {
        [SerializeField] private Selector<T> _timeSelector;
        [SerializeField] private EnvelopeFactory _envelopeFactory;
        
        public void EndTrigger(TrackedTriggerable triggerable, Guid id, T value)
        {
            IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            triggerable.EndTrigger(envelope, id, timeScale);
        }
    }

    public static class ValueTriggerSettingsExtensions
    {
        public static void Trigger<T>(this Triggerable triggerable, ValueTriggerSettings<T> settings, T value)
            where T : struct
        {
            settings.Trigger(triggerable, value);
        }

        public static Guid StartTrigger<T>(this TrackedTriggerable triggerable, ValueStartTriggerSettings<T> settings, T value)
            where T : struct
        {
            return settings.StartTrigger(triggerable, value);
        }

        public static void EndTrigger<T>(this TrackedTriggerable triggerable, ValueEndTriggerSettings<T> settings, Guid id, T value)
            where T : struct
        {
            settings.EndTrigger(triggerable, id, value);
        }
    }
    
}