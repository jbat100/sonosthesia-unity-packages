using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Envelope;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class PlayTriggerImplementation
    {
        private const float THRESHOLD = 1e-6f;
        
        private static readonly IEnvelope _defaultEnvelope =
            new ADSREnvelope(EnvelopePhase.InOutSine(0.3f), EnvelopePhase.InOutSine(0.5f), 
                0.5f, 0.5f, EnvelopePhase.InOutSine(0.5f));
        
        // used to avoid alloc on update
        private static readonly HashSet<Entry> _obsolete = new();

        private readonly HashSet<Entry> _entries = new ();
        private readonly AccumulationMode _accumulationMode;

        public PlayTriggerImplementation(AccumulationMode accumulationMode)
        {
            _accumulationMode = accumulationMode;
        }

        public void Clear()
        {
            _obsolete.Clear();
            _entries.Clear();
        }
        
        public void PlayTrigger(IEnvelope envelope, float valueScale, float timeScale)
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
            
            // Debug.Log($"{this} {nameof(PlayTrigger)} {nameof(valueScale)} {valueScale} {nameof(timeScale)} {timeScale}");

            Entry entry = new Entry(new WarpedEnvelope(envelope ?? _defaultEnvelope, valueScale, timeScale));
            _entries.Add(entry);
        }

        private class Entry  
        {
            private static float CurrentTime => Time.time;
            
            private readonly float _startTime;
            private readonly IEnvelope _envelope;
            private readonly float _endTime;

            public override string ToString()
            {
                return $"{nameof(Entry)} start {_startTime} end {_endTime}";
            }

            public Entry(IEnvelope envelope)
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

        public float Evaluate()
        {
            Purge();
            if (_entries.Count == 0)
            {
                return 0f;
            }
            return _entries.Aggregate(_accumulationMode.Seed(), 
                    (current, entry) => entry.Accumulate(_accumulationMode, current));
        }
        
        private void Purge()
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
        }
    }
}