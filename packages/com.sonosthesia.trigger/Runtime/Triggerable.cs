using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sonosthesia.Generator;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Trigger
{
    public class Triggerable : Signal<float>
    {
        [SerializeField] private FloatEnvelope _envelope;
        
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private FloatProcessor _postProcessor;

        public void Trigger(float valueScale, float timeScale)
        {
            TriggerEntry entry = new TriggerEntry(valueScale, timeScale, _envelope);
            _entries.Add(entry);
            // Debug.Log($"{this} added entry {entry}");
        }

        public int TriggerCount => _entries.Count; 

        // used to avoid alloc on update
        private static readonly HashSet<TriggerEntry> _obsolete = new();
        
        private readonly HashSet<TriggerEntry> _entries = new ();

        protected void Update()
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
            
            float raw = _entries.Aggregate(0f, (current, entry) => entry.Accumulate(_accumulationMode, current));
            Broadcast(_postProcessor.Process(raw));
        }
        
        private class TriggerEntry
        {
            private static float CurrentTime => Time.time;
            
            private readonly float _startTime;
            private readonly float _valueScale;
            private readonly float _timeScale;
            private readonly FloatEnvelope _envelope;
            private readonly float _endTime;

            public override string ToString()
            {
                return $"{nameof(TriggerEntry)} start {_startTime} value {_valueScale} time {_timeScale} end {_endTime}";
            }

            public TriggerEntry(float valueScale, float timeScale, FloatEnvelope envelope)
            {
                _startTime = CurrentTime;
                _valueScale = valueScale;
                _timeScale = timeScale;
                _envelope = envelope;
                _endTime = CurrentTime + envelope.Duration / timeScale;
            }

            public bool IsEnded => CurrentTime > _endTime;

            public float Accumulate(AccumulationMode accumulationMode, float current)
            {
                float value = _valueScale * _envelope.Evaluate((CurrentTime - _startTime) * _timeScale);;
                return accumulationMode switch
                {
                    AccumulationMode.Sum => current + value,
                    AccumulationMode.Max => Mathf.Max(current, value),
                    AccumulationMode.Min => Mathf.Min(current, value),
                    _ => 0
                };
            }
        }
        
    }
}