using System;
using System.Collections.Generic;
using Sonosthesia.Envelope;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    // like a TrackedTriggerImplementation but each entry has a weight and accumulation is non configurable
    // weighted sum
    
    public class WeightedTriggerImplementation : IDisposable
    {
        private readonly Dictionary<Guid, Entry> _entries = new ();

        // avoid alloc
        private static readonly HashSet<Guid> _obsolete = new();

        private static readonly IEnvelope _defaultStartEnvelope =
            new ADSEnvelope(EnvelopePhase.Linear(0.3f), EnvelopePhase.Linear(0.5f), 0.5f);
        
        private static readonly IEnvelope _defaultEndEnvelope =
            new SREnvelope(1f, EnvelopePhase.Linear(0.5f));
        
        private IDisposable _subscription;

        public float Reference { get; set; }
        
        public WeightedTriggerImplementation(float reference)
        {
            Reference = reference;
            _subscription = Observable.EveryUpdate().Subscribe(_ => Update());
        }

        public void Clear()
        {
            _entries.Clear();
            _obsolete.Clear();
        }

        public Guid StartTrigger(IEnvelope weightEnvelope, float value)
        {
            Guid id = Guid.NewGuid();
            StartTrigger(id, weightEnvelope, value);
            return id;
        }
        
        public void StartTrigger(Guid id, IEnvelope weightEnvelope, float value)
        {
            if (_entries.ContainsKey(id))
            {
                throw new ArgumentException($"Trigger with id {id} already exists");
            }
            _entries[id] = new Entry(weightEnvelope ?? _defaultStartEnvelope, value);
        }
        
        public bool UpdateTrigger(Guid id, float value)
        {
            if (_entries.TryGetValue(id, out Entry entry))
            {
                entry.Value = value;
                return true;
            }

            return false;
        }

        public bool EndTrigger(Guid id, IEnvelope weightEnvelope)
        {
            if (_entries.TryGetValue(id, out Entry entry))
            {
                entry.End(weightEnvelope ?? _defaultEndEnvelope);
                // note : don't remove from _entries, the end phase of the trigger must complete
                return true;
            }

            return false;
        }
        
        public void EndAll(IEnvelope weightEnvelope)
        {
            foreach (Entry entry in _entries.Values)
            {
                entry.End(weightEnvelope);
            }
        }

        public float Evaluate()
        {
            if (_entries.Count == 0)
            {
                return Reference;
            }

            float totalWeight = 1f;
            float totalValue = Reference;

            foreach (Entry entry in _entries.Values)
            {
                totalWeight += entry.Weight;
                totalValue += entry.Value * entry.Weight;
            }
            
            return totalValue / totalWeight;
        }
        
        private void Update()
        {
            _obsolete.Clear();
            foreach (KeyValuePair<Guid, Entry> pair in _entries)
            {
                if (pair.Value.IsEnded)
                {
                    _obsolete.Add(pair.Key);
                }
            }
            foreach (Guid id in _obsolete)
            {
                _entries.Remove(id);
            }
        }

        private class Entry
        {
            private class PhaseInfo
            {
                private static float CurrentTime => Time.time;
                
                private readonly float _referenceTime;
                private readonly IEnvelope _weightEnvelope;

                public PhaseInfo(IEnvelope weightEnvelope)
                {
                    _referenceTime = CurrentTime;
                    _weightEnvelope = weightEnvelope;
                }
                
                public bool IsComplete => CurrentTime - _referenceTime > _weightEnvelope.Duration;
                public float CurrentValue => IsComplete ? _weightEnvelope.FinalValue : _weightEnvelope.Evaluate(CurrentTime - _referenceTime);
            }

            private float _value;
            private readonly PhaseInfo _start;
            private PhaseInfo _end;

            private PhaseInfo CurrentPhase => _end ?? _start;
            
            public Entry(IEnvelope weightEnvelope, float value)
            {
                _value = value;
                _start = new PhaseInfo(weightEnvelope);
            }

            public float Value
            {
                get => _value;
                set => _value = value;
            }
            
            public float Weight => CurrentPhase.CurrentValue;

            /// <summary>
            /// Note value scale is computed automatically to allow smooth transition down from current value
            /// </summary>
            /// <param name="weightEnvelope"></param>
            public void End(IEnvelope weightEnvelope)
            {
                if (_end != null)
                {
                    return;
                }
                float weightScale = weightEnvelope.InitialValue == 0f ? 1f : _start.CurrentValue / weightEnvelope.InitialValue;
                _end = new PhaseInfo(new WarpedEnvelope(weightEnvelope, weightScale, 1f));
            }
            
            public bool IsEnded => _end is {IsComplete: true};
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}