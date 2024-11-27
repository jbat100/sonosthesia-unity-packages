using System;
using Sonosthesia.Envelope;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [Serializable]
    public class ValueTrigger<TValue> where TValue : struct
    {
        [SerializeField] private Selector<TValue> _valueSelector;

        [SerializeField] private Selector<TValue> _timeSelector;

        [SerializeField] private EnvelopeFactory _envelope;
        
        [SerializeField] private Trigger _trigger;

        public void Trigger(TValue value)
        {
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            _trigger.TriggerController.PlayTrigger(_envelope ? _envelope.Build() : null, valueScale, timeScale);
        }
    }
}