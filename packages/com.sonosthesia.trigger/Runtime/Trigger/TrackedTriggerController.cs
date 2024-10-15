using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sonosthesia.Envelope;

namespace Sonosthesia.Trigger
{
    public class TrackedTriggerController
    {
        private readonly Dictionary<Guid, Entry> _entries = new ();

        // avoid alloc
        private static readonly HashSet<Guid> _obsolete = new();

        private readonly AccumulationMode _accumulationMode;

        public TrackedTriggerController(AccumulationMode accumulationMode)
        {
            _accumulationMode = accumulationMode;
        }

        public void Clear()
        {
            _entries.Clear();
            _obsolete.Clear();
        }

        public void StartTrigger(Guid id, IEnvelope envelope, float valueScale, float timeScale)
        {
            if (_entries.ContainsKey(id))
            {
                throw new ArgumentException($"Trigger with id {id} already exists");
            }
            _entries[id] = new Entry(new WarpedEnvelope(envelope, valueScale, timeScale));
        }
        
        public bool UpdateTrigger(Guid id, float valueScale)
        {
            if (_entries.TryGetValue(id, out Entry entry))
            {
                entry.ValueScale = valueScale;
                return true;
            }

            return false;
        }

        public bool EndTrigger(Guid id, IEnvelope envelope, float timescale = 1f)
        {
            if (_entries.TryGetValue(id, out Entry entry))
            {
                if (Math.Abs(timescale - 1f) > 1e-6)
                {
                    envelope = new WarpedEnvelope(envelope, 1f, timescale);
                }
                entry.End(envelope);
                // note : don't remove from _entries, the end phase of the trigger must complete
                return true;
            }

            return false;
        }
        
        public void EndAll(IEnvelope envelope, float timescale = 1)
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

            float result = _entries.Values.Aggregate(0f, (current, entry) => entry.Accumulate(_accumulationMode, current));
            
            return result;
        }

        private class Entry
        {
            private class PhaseInfo
            {
                private static float CurrentTime => Time.time;
                
                public float ReferenceTime;
                public WarpedEnvelope Envelope;

                public bool IsComplete => CurrentTime - ReferenceTime > Envelope.Duration;

                public float CurrentValue
                {
                    get
                    {
                        if (IsComplete)
                        {
                            return Envelope.FinalValue;
                        }
                        return Envelope.Evaluate(CurrentTime - ReferenceTime);
                    }
                }
            }

            private readonly PhaseInfo _start;
            private PhaseInfo _end;

            private PhaseInfo CurrentPhase => _end ?? _start;
            
            public Entry(WarpedEnvelope envelope)
            {
                _start = new PhaseInfo
                {
                    ReferenceTime = Time.time,
                    Envelope = envelope
                };
            }

            public float ValueScale
            {
                get => _start.Envelope.ValueScale;
                set => _start.Envelope.ValueScale = value;
            }

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
                
                _end = new PhaseInfo
                {
                    ReferenceTime = Time.time,
                    Envelope = new WarpedEnvelope(envelope, valueScale, 1f)
                };
            }
            
            public bool IsEnded => _end is {IsComplete: true};

            public float Accumulate(AccumulationMode accumulationMode, float seed)
            {
                float value = CurrentPhase.CurrentValue;
                return accumulationMode switch
                {
                    AccumulationMode.Sum => seed + value,
                    AccumulationMode.Max => Mathf.Max(seed, value),
                    AccumulationMode.Min => Mathf.Min(seed, value),
                    _ => 0
                };
            }
        }
    }
}