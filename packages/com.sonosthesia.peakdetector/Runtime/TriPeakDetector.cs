using System;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.PeakDetector
{
    public class TriPeakDetector : MonoBehaviour
    {
        [SerializeField] private TriPeakDetectorConfiguration _configuration;

        [SerializeField] private Signal<TriBand<float>> _source;

        [SerializeField] private Signal<Peak> _lows;
        
        [SerializeField] private Signal<Peak> _mids;
        
        [SerializeField] private Signal<Peak> _highs;

        private PeakDetectorImplementation _lowsImplementation;
        private PeakDetectorImplementation _midsImplementation;
        private PeakDetectorImplementation _highsImplementation;

        private IDisposable _subscription;
        
        private void Setup()
        { 
            _subscription?.Dispose();
            
            _lowsImplementation = null;
            _midsImplementation = null;
            _highsImplementation = null;

            if (!_source || !_configuration)
            {
                return;
            }
            
            _lowsImplementation = _configuration.Lows.MakeImplementation(_lows.Broadcast);
            _midsImplementation = _configuration.Mids.MakeImplementation(_mids.Broadcast);
            _highsImplementation = _configuration.Highs.MakeImplementation(_highs.Broadcast);
            
            _subscription = _source.SignalObservable.Subscribe(tri =>
            {
                _lowsImplementation.Process(tri.Low);
                _midsImplementation.Process(tri.Mid);
                _highsImplementation.Process(tri.High);
            });
        }

        protected void OnEnable() => Setup();

        protected void OnValidate() => Setup();
    }
}