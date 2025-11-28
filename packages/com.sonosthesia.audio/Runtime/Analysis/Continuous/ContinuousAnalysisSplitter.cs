using System;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public class ContinuousAnalysisSplitter : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<ISignal<ContinuousAnalysis>> _source;

        [Header("Outputs")]
        
        [SerializeField] private InterfaceReference<ISignal<float>> _rms;
        [SerializeField] private InterfaceReference<ISignal<float>> _lows;
        [SerializeField] private InterfaceReference<ISignal<float>> _mids;
        [SerializeField] private InterfaceReference<ISignal<float>> _highs;
        [SerializeField] private InterfaceReference<ISignal<float>> _centroid;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source.Value != null)
            {
                _subscription = _source.Value.Observable.Subscribe(a =>
                {
                    _rms.Value?.Broadcast(a.rms);
                    _lows.Value?.Broadcast(a.lows);
                    _mids.Value?.Broadcast(a.mids);
                    _highs.Value?.Broadcast(a.highs);
                    _centroid.Value?.Broadcast(a.centroid);
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}