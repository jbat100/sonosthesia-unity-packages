using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [Serializable]
    public class ValueTriggerable<TValue> where TValue : struct
    {
        [SerializeField] private Selector<TValue> _valueSelector;

        [SerializeField] private Selector<TValue> _timeSelector;

        [SerializeField] private Triggerable _triggerable;

        public int TriggerCount => _triggerable.TriggerCount;
        
        public void Trigger(TValue value)
        {
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            _triggerable.Trigger(valueScale, timeScale);
        }
    }
}