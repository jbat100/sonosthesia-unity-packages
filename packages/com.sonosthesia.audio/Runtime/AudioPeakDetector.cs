using System;
using UniRx;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public readonly struct AudioPeak
    {
        public readonly float Magnitude;
        public readonly float Duration;

        public AudioPeak(float magnitude, float duration)
        {
            Magnitude = magnitude;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"AudioPeak (Magnitude : {Magnitude}, Duration : {Duration})";
        }
    }
    
    public class AudioPeakDetector : Adaptor<float, AudioPeak>
    {
        [SerializeField] private AudioPeakDetectorSettings _settings;
        
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
        
        private Sample? _start;
        private Sample? _previous;
        
        protected override IDisposable Setup(Signal<float> source) => source.SignalObservable.Subscribe(Process);

        protected override void OnEnable()
        {
            _start = _previous = null;
            base.OnEnable();
        }

        private void Process(float value)
        {
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
                    Broadcast(new AudioPeak(magnitude, duration));
                }
                _start = null;
            }
            
            _previous = new Sample(value);
        }
    }
}