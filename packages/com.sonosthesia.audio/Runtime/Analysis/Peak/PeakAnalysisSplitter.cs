using System;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.Audio
{
    public class PeakAnalysisSplitter : MonoBehaviour
    {
        [SerializeField] private Signal<PeakAnalysis> _source;

        [Header("Outputs")] 
        
        [SerializeField] private Signal<Peak> _main;
        [SerializeField] private Signal<Peak> _lows;
        [SerializeField] private Signal<Peak> _mids;
        [SerializeField] private Signal<Peak> _highs;

        private IDisposable _subscription;

        protected virtual Signal<Peak> SignalForChannel(int channel) => channel switch
        {
            0 => _main,
            1 => _lows,
            2 => _mids,
            3 => _highs,
            _ => null
        };

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.SignalObservable.Subscribe(analysis =>
                {
                    Signal<Peak> signal = SignalForChannel(analysis.channel);
                    if (signal)
                    {
                        signal.Broadcast(analysis.ToPeak());
                    }
                });
            }
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
        }
    }
}