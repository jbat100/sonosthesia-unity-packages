using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public class ContinuousAnalysisSplitter : MonoBehaviour
    {
        [SerializeField] private Signal<ContinuousAnalysis> _source;

        [Header("Outputs")]
        
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
                    if (_rms)
                    {
                        _rms.Broadcast(a.rms);
                    }
                    if (_lows)
                    {
                        _lows.Broadcast(a.lows);    
                    }
                    if (_mids)
                    {
                        _mids.Broadcast(a.mids);    
                    }
                    if (_highs)
                    {
                        _highs.Broadcast(a.highs);    
                    }
                    if (_centroid)
                    {
                        _centroid.Broadcast(a.centroid);    
                    }
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}