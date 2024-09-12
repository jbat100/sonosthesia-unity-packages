using System;
using Sonosthesia.Pack;
using Sonosthesia.Signal;
using Sonosthesia.Trigger;
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
        
        [SerializeField] private Signal<Peak> _peaks;

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
                    if (a.offset && _peaks)
                    {
                        // audio analysis contains no info on peaks, just offset detection
                        // note it probably didn't have much meaning anyway
                        _peaks.Broadcast(new Peak(1f, 1f));
                    }
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}