using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Pack
{
    public abstract class AudioBandSplitter<T> : MonoBehaviour where T : IPackedAudioBands
    {
        [SerializeField] private PackAudioBandReceiver<T> _receiver;

        [SerializeField] private string _track;
        
        private IDisposable _subscription;

        protected virtual void Awake()
        {
            if (!_receiver)
            {
                _receiver = GetComponentInParent<PackAudioBandReceiver<T>>();
            }
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _receiver.TrackBandObservable(_track)
                .Subscribe(bands =>
                {
                    for (int i = 0; i < bands.BandCount; i++)
                    {
                        Signal<float> signal = GetSignal(i);
                        if (signal)
                        {
                            signal.Broadcast(bands.GetBand(i));
                        }
                    }
                });
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

        private protected abstract Signal<float> GetSignal(int index);
    }
}