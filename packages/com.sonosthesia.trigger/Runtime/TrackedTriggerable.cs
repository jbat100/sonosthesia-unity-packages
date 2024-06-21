using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Generator;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TrackedTriggerable))]
    public class TrackedTriggerableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TrackedTriggerable triggerable = (TrackedTriggerable)target;
            if(GUILayout.Button("End All"))
            {
                triggerable.EndAll();
            }
        }
    }
#endif
    
    public class TrackedTriggerable : Signal<float>
    {
        [SerializeField] private FloatEnvelope _startEnvelope;
        
        [SerializeField] private FloatEnvelope _endEnvelope;
        
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private FloatProcessor _postProcessor;

        private readonly Dictionary<Guid, TriggerEntry> _entries = new ();

        // avoid alloc
        private static readonly HashSet<Guid> _obsolete = new();

        public Guid StartTrigger(float valueScale, float timeScale)
        {
            Guid id = Guid.NewGuid();
            _entries[id] = new TriggerEntry(valueScale, timeScale, _startEnvelope);
            return id;
        }

        public void UpdateTrigger(Guid id, float valueScale)
        {
            if (_entries.TryGetValue(id, out TriggerEntry entry))
            {
                entry.Update(valueScale);
            }
        }

        public void EndTrigger(Guid id)
        {
            if (_entries.TryGetValue(id, out TriggerEntry entry))
            {
                entry.End(_endEnvelope);
            }
        }
        
        public void EndTrigger(Guid id, float timeScale)
        {
            if (_entries.TryGetValue(id, out TriggerEntry entry))
            {
                entry.End(timeScale, _endEnvelope);
            }
        }

        public void EndAll()
        {
            foreach (TriggerEntry entry in _entries.Values)
            {
                entry.End(_endEnvelope);
            }
        }
        
        public void EndAll(float timeScale)
        {
            foreach (TriggerEntry entry in _entries.Values)
            {
                entry.End(timeScale, _endEnvelope);
            }
        }
        
        protected virtual void Update()
        {
            _obsolete.Clear();
            foreach (KeyValuePair<Guid, TriggerEntry> pair in _entries)
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

            float raw = _entries.Values.Aggregate(0f, (current, entry) => entry.Accumulate(_accumulationMode, current));
            Broadcast(_postProcessor.Process(raw));
        }

        protected virtual void OnEnable()
        {
            _entries.Clear();
        }
        
        protected virtual void OnDisable()
        {
            _entries.Clear();
        }

        private class TriggerEntry
        {
            private class PhaseInfo
            {
                private static float CurrentTime => Time.time;
                
                public float ReferenceTime;
                public float ValueScale;
                public float TimeScale;
                public FloatEnvelope Envelope;

                public bool IsComplete => CurrentTime - ReferenceTime > Envelope.Duration * TimeScale;

                public float CurrentValue
                {
                    get
                    {
                        if (IsComplete)
                        {
                            return ValueScale * Envelope.FinalValue;;
                        }
                        return ValueScale * Envelope.Evaluate((CurrentTime - ReferenceTime) / TimeScale);
                    }
                }
            }

            private readonly PhaseInfo _start;
            private PhaseInfo _end;

            private PhaseInfo CurrentPhase => _end ?? _start;
            
            public TriggerEntry(float valueScale, float timeScale, FloatEnvelope envelope)
            {
                _start = new PhaseInfo
                {
                    ReferenceTime = Time.time,
                    Envelope = envelope,
                    ValueScale = valueScale,
                    TimeScale = timeScale
                };
            }

            public void Update(float valueScale)
            {
                _start.ValueScale = valueScale;
            }
            
            public void End(FloatEnvelope envelope) => End(_start.TimeScale, envelope);

            /// <summary>
            /// Note value scale is computed automatically to allow smooth transition down from current value
            /// </summary>
            /// <param name="timeScale"></param>
            /// <param name="envelope"></param>
            public void End(float timeScale, FloatEnvelope envelope)
            {
                if (_end != null)
                {
                    return;
                }

                _end = new PhaseInfo
                {
                    ReferenceTime = Time.time,
                    Envelope = envelope,
                    ValueScale = envelope.InitialValue == 0f ? 1f : _start.CurrentValue / envelope.InitialValue,
                    TimeScale = timeScale
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