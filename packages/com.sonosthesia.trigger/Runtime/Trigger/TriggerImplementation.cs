using System;
using System.Collections.Generic;
using Sonosthesia.Envelope;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    // like a TrackedTriggerImplementation but each entry has a weight and accumulation is non configurable
    // weighted sum
    
    public class TriggerImplementation
    {
        private readonly AccumulationMode _accumulationMode;
        
        private readonly Dictionary<Guid, Entry> _entries = new ();

        private static readonly HashSet<Guid> _obsolete = new();

        private static readonly IEnvelope _defaultStartEnvelope =
            new ADSEnvelope(EnvelopePhase.Linear(0.3f), EnvelopePhase.Linear(0.5f), 0.5f);
        
        private static readonly IEnvelope _defaultEndEnvelope =
            new SREnvelope(1f, EnvelopePhase.Linear(0.5f));

        public TriggerImplementation(AccumulationMode accumulationMode)
        {
            _accumulationMode = accumulationMode;
        }
        
        public void Clear()
        {
            _entries.Clear();
        }

        public Guid StartTrigger(IEnvelope envelope, float value, float attack, bool autoRelease)
        {
            Guid id = Guid.NewGuid();
            StartTrigger(id, envelope, value, attack, autoRelease);
            return id;
        }
        
        public void StartTrigger(Guid id, IEnvelope envelope, float value, float attack, bool autoRelease)
        {
            if (_entries.ContainsKey(id))
            {
                throw new ArgumentException($"Trigger with id {id} already exists");
            }
            envelope ??= _defaultStartEnvelope;
            _entries[id] = new Entry(new WarpedEnvelope(envelope, 1f, attack), value, autoRelease);
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

        public bool EndTrigger(Guid id, IEnvelope envelope, float release)
        {
            if (_entries.TryGetValue(id, out Entry entry))
            {
                entry.End(new WarpedEnvelope(envelope ?? _defaultEndEnvelope, 1f, release));
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
            Purge();

            if (_entries.Count == 0)
            {
                return 0f;
            }

            return _accumulationMode switch
            {
                AccumulationMode.None => 0,
                AccumulationMode.Sum => EvaluateSum(),
                AccumulationMode.Max => EvaluateMax(),
                AccumulationMode.Min => EvaluateMin(),
                AccumulationMode.Weighted => EvaluateWeighted(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float EvaluateSum()
        {
            float sum = 0f;
            foreach (Entry entry in _entries.Values)
            {
                sum += entry.Value * entry.Weight;
            }
            return sum;
        }
        
        private float EvaluateMax()
        {
            float max = float.NegativeInfinity;
            foreach (Entry entry in _entries.Values)
            {
                float value = entry.Value * entry.Weight;
                if (value > max)
                {
                    max = value;
                }
            }
            return max;
        }
        
        private float EvaluateMin()
        {
            float min = float.PositiveInfinity;
            foreach (Entry entry in _entries.Values)
            {
                float value = entry.Value * entry.Weight;
                if (value < min)
                {
                    min = value;
                }
            }
            return min;
        }
        
        private float EvaluateWeighted()
        {
            float totalWeight = 0;
            float totalValue = 0;
            foreach (Entry entry in _entries.Values)
            {
                totalWeight += entry.Weight;
                totalValue += entry.Value * entry.Weight;
            }
            return totalWeight != 0 ? totalValue / totalWeight : 0f;
        }
        
        private void Purge()
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
                private readonly IEnvelope _envelope;

                public PhaseInfo(IEnvelope envelope)
                {
                    _referenceTime = CurrentTime;
                    _envelope = envelope;
                }
                
                public bool IsComplete => CurrentTime - _referenceTime > _envelope.Duration;
                public float Evaluate() => IsComplete ? _envelope.FinalValue : _envelope.Evaluate(CurrentTime - _referenceTime);
            }

            private readonly bool _autoRelease;
            private readonly PhaseInfo _start;
            private PhaseInfo _end;

            private PhaseInfo CurrentPhase => _end ?? _start;
            
            public Entry(IEnvelope weightEnvelope, float value, bool autoRelease)
            {
                Value = value;
                _autoRelease = autoRelease;
                _start = new PhaseInfo(weightEnvelope);
            }

            public float Value { get; set; }

            public float Weight => CurrentPhase.Evaluate();

            /// <summary>
            /// Note value scale is computed automatically to allow smooth transition down from current value
            /// </summary>
            /// <param name="envelope"></param>
            public void End(IEnvelope envelope)
            {
                if (_autoRelease || _end != null)
                {
                    return;
                }
                float weightScale = envelope.InitialValue == 0f ? 1f : _start.Evaluate() / envelope.InitialValue;
                _end = new PhaseInfo(new WarpedEnvelope(envelope, weightScale, 1f));
            }
            
            public bool IsEnded => (_autoRelease && _start is {IsComplete: true}) || _end is {IsComplete: true};
        }
    }
}