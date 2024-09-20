using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;
using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{
    public class AudioBandFloatSignal<T> : Signal<float> where T : IPackedAudioBands
    {
        [SerializeField] private PackAudioBandReceiver<T> _receiver;

        [SerializeField] private string _track;
        
        [SerializeField] private int _index = 1;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _receiver.TrackBandObservable(_track)
                .Select(bands => bands.GetBand(_index))
                .Subscribe(Broadcast);
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}