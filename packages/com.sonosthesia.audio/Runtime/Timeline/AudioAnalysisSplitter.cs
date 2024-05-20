using System;
using Sonosthesia.Pack;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public class AudioAnalysisSplitter : MonoBehaviour
    {
        [SerializeField] private Signal<AudioAnalysis> _source;

        [SerializeField] private Signal<float> _rms;
        
        [SerializeField] private Signal<float> _lows;
        
        [SerializeField] private Signal<float> _mids;
        
        [SerializeField] private Signal<float> _highs;
        
        [SerializeField] private Signal<float> _centroid;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.SignalObservable.Subscribe(a =>
                {
                    _rms.Broadcast(a.rms);
                    _lows.Broadcast(a.lows);
                    _mids.Broadcast(a.mids);
                    _highs.Broadcast(a.highs);
                    _centroid.Broadcast(a.centroid);
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}