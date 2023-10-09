using System;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class AudioQuintBandFloatSignal : Signal<float>
    {
        [SerializeField] private PackAudioBandReceiver _receiver;

        [SerializeField] private string _track;
        
        [SerializeField, Range(1, 5)] private int _index = 1;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _receiver.QuintBandObservable
                .Where(band => band.Track == _track)
                .Select(bands => bands.GetBand(_index))
                .Subscribe(Broadcast);
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}