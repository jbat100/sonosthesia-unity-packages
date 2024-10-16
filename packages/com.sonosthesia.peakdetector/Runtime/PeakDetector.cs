using System;
using UnityEngine;
using UniRx;
using Sonosthesia.Signal;
using Sonosthesia.Flow;
using Sonosthesia.Processing;
using Sonosthesia.Utils;

namespace Sonosthesia.PeakDetector
{
    internal class PeakDetectorImplementation
    {
        private readonly struct Sample
        {
            public readonly float Value;
            public readonly float Time;

            public Sample(float value)
            {
                Value = value;
                Time = UnityEngine.Time.time;
            }
        }
        
        private readonly IDynamicProcessor<float> _preprocessor;
        private readonly PeakDetectorSettings _settings;
        private readonly Action<Peak> _broadcast;
        private readonly bool _log;
        
        private Sample? _start;
        private Sample? _previous;
        
        public PeakDetectorImplementation(IDynamicProcessor<float> preprocessor, PeakDetectorSettings settings, Action<Peak> broadcast, bool log = false)
        {
            _preprocessor = preprocessor;
            _settings = settings;
            _broadcast = broadcast;
            _log = log;
        }

        public void Reset()
        {
            _start = _previous = null;
        }
        
        public void Process(float value)
        {
            value = _preprocessor?.Process(value, Time.time) ?? value;
            
            if (!_previous.HasValue)
            {
                _previous = new Sample(value);
                return;
            }

            if (!_start.HasValue)
            {
                if (value > _previous.Value.Value)
                {
                    _start = _previous;   
                }
                _previous = new Sample(value);
                return;
            }

            // here we have a start if descending, then check if we have a been through a peak 
            if (value < _previous.Value.Value)
            {
                // check if we have a peak between start and previous 
                float magnitude = _previous.Value.Value - _start.Value.Value;
                float duration = _previous.Value.Time - _start.Value.Time;
                if (magnitude > _settings.MagnitudeThreshold && duration < _settings.MaximumDuration)
                {
                    if (_log)
                    {
                        Debug.LogWarning($"{this} broadcasting peak {nameof(magnitude)} {magnitude} {nameof(duration)} {duration}");   
                    }
                    _broadcast(new Peak(_settings.ValuePostProcessor.Process(magnitude), duration));
                }
                else
                {
                    if (_log)
                    {
                        Debug.Log($"{this} no peak detected {nameof(magnitude)} {magnitude} {nameof(duration)} {duration}");   
                    }
                }
                _start = null;
            }
            
            _previous = new Sample(value);
        }
    }
    
    // note : Had issues when assigning PeakDetector to a Signal<Peak> field in inspector
    // solution was to drag the component itself (using dual inspector tab with one locked)
    
    public class PeakDetector : Adaptor<float, Peak>
    {
        [SerializeField] private DynamicProcessorFactory<float> _preprocessorFactory;

        [SerializeField] private BasePeakDetectorConfiguration _settings;

        private PeakDetectorImplementation _implementation;
        
        protected override IDisposable Setup(Signal<float> source) => source.SignalObservable.Subscribe(value => _implementation?.Process(value));

        protected override void OnEnable()
        {
            RefreshImplementation();
            base.OnEnable();
        }

        protected virtual void OnValidate() => RefreshImplementation();

        private void RefreshImplementation()
        {
            _implementation = new PeakDetectorImplementation(
                _preprocessorFactory ? _preprocessorFactory.Make() : null, 
                _settings ? _settings.Settings : null, 
                Broadcast);
        }
    }
}