using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Envelope;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BuilderTrackedTriggerable))]
    public class TrackedTriggerableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BuilderTrackedTriggerable triggerable = (BuilderTrackedTriggerable)target;
            if(GUILayout.Button("End All"))
            {
                triggerable.EndAll();
            }
        }
    }
#endif
    
    public class BuilderTrackedTriggerable : Signal<float>
    {
        [SerializeField] private EnvelopeBuilder _startEnvelopeBuilder;
        
        [SerializeField] private EnvelopeBuilder _endEnvelopeBuilder;
        
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private FloatProcessor _postProcessor;

        private readonly Dictionary<Guid, TriggerEntry> _entries = new ();

        // avoid alloc
        private static readonly HashSet<Guid> _obsolete = new();

        public Guid StartTrigger(float valueScale, float timeScale)
        {
            Guid id = Guid.NewGuid();
            _entries[id] = new TriggerEntry(new WarpedEnvelope(_startEnvelopeBuilder.Build(), valueScale, timeScale));
            return id;
        }

        public void EndTrigger(Guid id)
        {
            if (_entries.TryGetValue(id, out TriggerEntry entry))
            {
                entry.End(_endEnvelopeBuilder.Build());
            }
        }
        
        public void EndTrigger(Guid id, float timeScale)
        {
            if (_entries.TryGetValue(id, out TriggerEntry entry))
            {
                entry.End(new WarpedEnvelope(_endEnvelopeBuilder.Build(), 1f, timeScale));
            }
        }

        public void EndAll()
        {
            IEnvelope envelope = _endEnvelopeBuilder.Build();
            foreach (TriggerEntry entry in _entries.Values)
            {
                entry.End(envelope);
            }
        }
        
        public void EndAll(float timeScale)
        {
            IEnvelope envelope = new WarpedEnvelope(_endEnvelopeBuilder.Build(), 1f, timeScale);
            foreach (TriggerEntry entry in _entries.Values)
            {
                entry.End(envelope);
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
            Broadcast(0);
        }

        private class TriggerEntry
        {
            private class PhaseInfo
            {
                private static float CurrentTime => Time.time;
                
                public float ReferenceTime;
                public IEnvelope Envelope;

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
            
            public TriggerEntry(IEnvelope envelopeBuilder)
            {
                _start = new PhaseInfo
                {
                    ReferenceTime = Time.time,
                    Envelope = envelopeBuilder
                };
            }


            /// <summary>
            /// Note value scale is computed automatically to allow smooth transition down from current value
            /// </summary>
            /// <param name="timeScale"></param>
            /// <param name="envelopeBuilder"></param>
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