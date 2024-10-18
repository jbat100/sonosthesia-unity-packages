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

        public void Trigger(Trigger trigger, T value)
        {
            IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            trigger.TriggerController.PlayTrigger(envelope, valueScale, timeScale);
        }
    }

    [Serializable]
    public class ValueStartTriggerSettings<T> where T : struct
    {
        [SerializeField] private Selector<T> _valueSelector;
        [SerializeField] private Selector<T> _timeSelector;
        [SerializeField] private EnvelopeFactory _envelopeFactory;

        public Guid StartTrigger(Trigger trigger, T value)
        {
            IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            return trigger.TriggerController.StartTrigger(envelope, valueScale, timeScale);
        }
    }
    
    [Serializable]
    public class ValueEndTriggerSettings<T> where T : struct
    {
        [SerializeField] private Selector<T> _timeSelector;
        [SerializeField] private EnvelopeFactory _envelopeFactory;
        
        public void EndTrigger(Trigger trigger, Guid id, T value)
        {
            IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            trigger.TriggerController.EndTrigger(id, envelope, timeScale);
        }
    }

    public static class ValueTriggerSettingsExtensions
    {
        public static void Trigger<T>(this Trigger trigger, ValueTriggerSettings<T> settings, T value)
            where T : struct
        {
            settings.Trigger(trigger, value);
        }

        public static Guid StartTrigger<T>(this Trigger trigger, ValueStartTriggerSettings<T> settings, T value)
            where T : struct
        {
            return settings.StartTrigger(trigger, value);
        }

        public static void EndTrigger<T>(this Trigger trigger, ValueEndTriggerSettings<T> settings, Guid id, T value)
            where T : struct
        {
            settings.EndTrigger(trigger, id, value);
        }
    }
    
}