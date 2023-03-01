using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public enum AccumulationMode
    {
        None,
        Sum,
        Max,
        Min
    }
    
    public class TriggerFloatSignal : Signal<float>
    {
        [SerializeField] private FloatEnvelope _envelope;
        
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        public void Trigger(float valueScale, float timeScale)
        {
            _entries.Add(new TriggerEntry(Time.time, valueScale, timeScale, _envelope));
        }
        
        protected void Update()
        {
            _entries.ExceptWith(_entries.Where(entry => entry.Ended(Time.time)).ToList());
            Broadcast(_entries.Aggregate(0f, (current, entry) => entry.Accumulate(Time.time, _accumulationMode, current)));
        }
        
        private class TriggerEntry
        {
            private float _startTime;
            private float _valueScale;
            private float _timeScale;
            private FloatEnvelope _envelope;
            private float _fadeTime;
            private float _endTime;
            
            public TriggerEntry(float startTime, float valueScale, float timeScale, FloatEnvelope envelope)
            {
                _startTime = startTime;
                _valueScale = valueScale;
                _timeScale = timeScale;
                _envelope = envelope;
                _endTime = envelope.Duration() * timeScale;
            }

            public bool Ended(float time) => time > _endTime;

            public float Accumulate(float time, AccumulationMode accumulationMode, float current)
            {
                float value = _valueScale * _envelope.Evaluate((time - _startTime) / _timeScale);;
                return accumulationMode switch
                {
                    AccumulationMode.Sum => current + value,
                    AccumulationMode.Max => Mathf.Max(current, value),
                    AccumulationMode.Min => Mathf.Min(current, value),
                    _ => throw new NotImplementedException()
                };
            }
        }
        
        private readonly HashSet<TriggerEntry> _entries = new ();
    }
}