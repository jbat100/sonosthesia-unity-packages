using System;
using System.Collections.Generic;
using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    // like a TrackedTriggerImplementation but each entry has a weight and accumulation is non configurable
    // weighted sum
    
    public class WeightedTriggerImplementation
    {
        private readonly Dictionary<Guid, Entry> _entries = new ();

        // avoid alloc
        private static readonly HashSet<Guid> _obsolete = new();

        private static readonly IEnvelope _defaultStartEnvelope =
            new ADSEnvelope(EnvelopePhase.Linear(0.3f), EnvelopePhase.Linear(0.5f), 0.5f);
        
        private static readonly IEnvelope _defaultEndEnvelope =
            new SREnvelope(1f, EnvelopePhase.Linear(0.5f));

        public float Reference { get; set; }
        
        public WeightedTriggerImplementation(float reference)
        {
            Reference = reference;
        }

        public void Clear()
        {
            _entries.Clear();
            _obsolete.Clear();
        }

        public Guid StartTrigger(IEnvelope envelope, float value)
        {
            Guid id = Guid.NewGuid();
            StartTrigger(id, envelope, value);
            return id;
        }
        
        public void StartTrigger(Guid id, IEnvelope envelope, float value)
        {
            if (_entries.ContainsKey(id))
            {
                throw new ArgumentException($"Trigger with id {id} already exists");
            }
            _entries[id] = new Entry(envelope ?? _defaultStartEnvelope, value);
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

        public bool EndTrigger(Guid id, IEnvelope envelope)
        {
            if (_entries.TryGetValue(id, out Entry entry))
            {
                entry.End(envelope);
                // note : don't remove from _entries, the end phase of the trigger must complete
                return true;
            }

            return false;
        }
        
        public void EndAll(IEnvelope envelope)
        {
            foreach (Entry entry in _entries.Values)
            {
                entry.End(envelope);
            }
        }
        
        public float Update()
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

        private class Entry
        {
            private class PhaseInfo
            {
                private static float CurrentTime => Time.time;
                
                private readonly float _referenceTime;
                private readonly IEnvelope _envelope;

                public PhaseInfo(IEnvelope envelope)
                {
                    _referenceTime = CurrentTime;
                    _envelope = envelope;
                }
                
                public bool IsComplete => CurrentTime - _referenceTime > _envelope.Duration;
                public float CurrentValue => IsComplete ? _envelope.FinalValue : _envelope.Evaluate(CurrentTime - _referenceTime);
            }

            private float _value;
            private readonly PhaseInfo _start;
            private PhaseInfo _end;

            private PhaseInfo CurrentPhase => _end ?? _start;
            
            public Entry(IEnvelope envelope, float value)
            {
                _value = value;
                _start = new PhaseInfo(envelope);
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
            /// <param name="envelope"></param>
            public void End(IEnvelope envelope)
            {
                if (_end != null)
                {
                    return;
                }
                float valueScale = envelope.InitialValue == 0f ? 1f : _start.CurrentValue / envelope.InitialValue;
                _end = new PhaseInfo(new WarpedEnvelope(envelope, valueScale, 1f));
            }
            
            public bool IsEnded => _end is {IsComplete: true};
        }
    }
}