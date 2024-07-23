using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceFade : MonoBehaviour
    {
        [SerializeField] private FloatSignalRelay _fade;
        
        private AudioSource _audioSource;
        private IDisposable _subscription;

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_fade && _audioSource)
            {
                _fade.Observable.Subscribe(f =>
                {
                    _audioSource.volume = 1f - f;
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}