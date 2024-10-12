using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class TriggerController : IDisposable
    {
        private const float THRESHOLD = 1e-6f;
        
        // used to avoid alloc on update
        private static readonly HashSet<TriggerEntry> _obsolete = new();

        private readonly HashSet<TriggerEntry> _entries = new ();
        private readonly AccumulationMode _accumulationMode;
        
        public TriggerController(AccumulationMode accumulationMode)
        {
            _accumulationMode = accumulationMode;
        }

        public void Clear()
        {
            _obsolete.Clear();
            _entries.Clear();
        }
        
        public void StartTrigger(IEnvelope envelope, float valueScale, float timeScale)
        {
            if (Mathf.Abs(valueScale) < THRESHOLD)
            {
                return;
            }
            if (timeScale < THRESHOLD)
            {
                Debug.LogWarning($"{this} fired with tiny time scale {timeScale}");
                return;
            }

            TriggerEntry entry = new TriggerEntry(new WarpedEnvelope(envelope, valueScale, timeScale));
            _entries.Add(entry);
        }

        public int TriggerCount => _entries.Count;
        
        private class TriggerEntry  
        {
            private static float CurrentTime => Time.time;
            
            private readonly float _startTime;
            private readonly IEnvelope _envelope;
            private readonly float _endTime;

            public override string ToString()
            {
                return $"{nameof(TriggerEntry)} start {_startTime} end {_endTime}";
            }

            public TriggerEntry(IEnvelope envelope)
            {
                _startTime = CurrentTime;
                _envelope = envelope;
                _endTime = CurrentTime + envelope.Duration;
            }

            public bool IsEnded => CurrentTime > _endTime;

            public float Accumulate(AccumulationMode accumulationMode, float current)
            {
                float value = _envelope.Evaluate((CurrentTime - _startTime));
                return accumulationMode switch
                {
                    AccumulationMode.Sum => current + value,
                    AccumulationMode.Max => Mathf.Max(current, value),
                    AccumulationMode.Min => Mathf.Min(current, value),
                    _ => 0
                };
            }
        }
        
        public float Update()
        {
            int previousCount = _entries.Count;
            _obsolete.Clear();
            _obsolete.UnionWith(_entries.Where(entry => entry.IsEnded));
            _entries.ExceptWith(_obsolete);
            _obsolete.Clear();
            int currentCount = _entries.Count;
            
            if (previousCount != currentCount)
            {
                // Debug.Log($"{this} removed {previousCount - currentCount} obsolete entries");
            }
            
            float result = _entries.Aggregate(0f, (current, entry) => entry.Accumulate(_accumulationMode, current));

            return result;
        }
        
        public void Dispose()
        {
            Clear();
        }
    }
}