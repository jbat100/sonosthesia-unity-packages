using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Pack
{
    public class AudioTriBandFloatSignal : Signal<float>
    {
        [SerializeField] private PackAudioBandReceiver _receiver;

        [SerializeField] private string _track;
        
        [SerializeField, Range(1, 3)] private int _index = 1;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _receiver.TriBandObservable
                .Where(band => band.Track == _track)
                .Select(bands => bands.GetBand(_index))
                .Subscribe(Broadcast);
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}