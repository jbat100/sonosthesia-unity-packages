using System;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.Audio
{
    public class PeakAnalysisSplitter : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<ISignal<PeakAnalysis>> _source;

        [Header("Outputs")] 
        
        [SerializeField] private  InterfaceReference<ISignal<Peak>> _main;
        [SerializeField] private  InterfaceReference<ISignal<Peak>> _lows;
        [SerializeField] private  InterfaceReference<ISignal<Peak>> _mids;
        [SerializeField] private  InterfaceReference<ISignal<Peak>> _highs;

        private IDisposable _subscription;

        protected virtual ISignal<Peak> SignalForChannel(int channel) => channel switch
        {
            0 => _main.Value,
            1 => _lows.Value,
            2 => _mids.Value,
            3 => _highs.Value,
            _ => null
        };

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source.Value != null)
            {
                _subscription = _source.Value.Observable.Subscribe(analysis =>
                {
                    SignalForChannel(analysis.channel)?.Broadcast(analysis.ToPeak());
                });
            }
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
        }
    }
}