using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [Serializable]
    public class ValueTrigger<TValue> where TValue : struct
    {
        [SerializeField] private Selector<TValue> _valueSelector;

        [SerializeField] private Selector<TValue> _timeSelector;

        [SerializeField] private BuilderTrigger trigger;

        public int TriggerCount => trigger.TriggerCount;
        
        public void Trigger(TValue value)
        {
            float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
            trigger.StartTrigger(valueScale, timeScale);
        }
    }
}