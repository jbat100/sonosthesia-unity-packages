using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Envelope;
using Sonosthesia.Processing;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class Triggerable : Signal<float>
    {
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private DynamicProcessorFactory<float> _postProcessorFactory;

        [SerializeField] private FloatProcessor _postProcessor;

        private const float THRESHOLD = 1e-6f;

        private IDynamicProcessor<float> _dynamicPostProcessor;

        private IEnvelope _defaultEnvelope;
        protected virtual IEnvelope DefaultEnvelope =>
            _defaultEnvelope ??= new AHREnvelope(EnvelopePhase.Linear(0.25f), 0.5f, EnvelopePhase.Linear(0.25f));

        [Serializable]
        public class Payload
        {
            [SerializeField] private float _timeScale = 1f;
            public float TimeScale => _timeScale;
            
            [SerializeField] private float _valueScale = 1f;
            public float ValueScale => _valueScale;
        }
        
        /// <summary>
        /// Useful for inspector bindings in Timeline etc...
        /// Note : should use simple custom Track like in the case of Sonosthesia.Trajectory
        /// </summary>
        /// <param name="payload"></param>
        public void PayloadTrigger(Payload payload)
        {
            if (payload == null)
            {
                return;
            }
            Trigger(payload.ValueScale, payload.TimeScale);
        }
        
        public void DefaultTrigger()
        {
            Trigger(1f, 1f);
        }

        public void Trigger(float valueScale, float timeScale) => Trigger(DefaultEnvelope, valueScale, timeScale);

        public void Trigger(IEnvelope envelope, float valueScale, float timeScale)
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

            envelope ??= DefaultEnvelope;
            
            TriggerEntry entry = new TriggerEntry(new WarpedEnvelope(envelope, valueScale, timeScale));
            _entries.Add(entry);
            // Debug.Log($"{this} added entry {entry}");
        }

        public int TriggerCount => _entries.Count; 

        // used to avoid alloc on update
        private static readonly HashSet<TriggerEntry> _obsolete = new();
        
        private readonly HashSet<TriggerEntry> _entries = new ();

        private void SetupState()
        {
            _obsolete.Clear();
            _entries.Clear();
            _dynamicPostProcessor = _postProcessorFactory ? _postProcessorFactory.Make() : null;
        }

        protected virtual void OnValidate() => SetupState();

        protected virtual void OnEnable() => SetupState();
        
        protected virtual void Update()
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

            result = _postProcessor.Process(result);

            if (_dynamicPostProcessor != null)
            {
                result = _dynamicPostProcessor.Process(result, Time.time);
            }
            
            Broadcast(_postProcessor.Process(result));
        }
        
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
    }
}