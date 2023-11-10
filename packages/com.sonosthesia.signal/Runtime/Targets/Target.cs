using System;
using Sonosthesia.Processing;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Signal
{
    public abstract class Target<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _source;

        [SerializeField] private DynamicProcessorFactory<T> _processingFactory;

        private IDisposable _subscription;
        private IDynamicProcessor<T> _processor;

        protected virtual void Awake()
        {
            if (!_source)
            {
                _source = GetComponent<Signal<T>>();
            }
        }
        
        protected void OnEnable()
        {
            _processor = _processingFactory ? _processingFactory.Make() : null;
            _subscription?.Dispose();

            if (_processor != null)
            {
                float startTime = Time.time;
                _subscription = _source.SignalObservable.Subscribe(value =>
                {
                    Apply(_processor.Process(value, Time.time - startTime));
                });    
            }
            else
            {
                _subscription = _source.SignalObservable.Subscribe(Apply); 
            }
        }

        protected void OnDisable()
        {
            _processor = null;
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void Apply(T value);
    }
}