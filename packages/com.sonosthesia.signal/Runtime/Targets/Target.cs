using System;
using Sonosthesia.Processing;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Signal
{
    public abstract class Target<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _source;

        [SerializeField] private bool _distinct = true; 

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
        
        protected virtual void OnEnable() 
        {
            _subscription?.Dispose();
            _processor = null;
            
            if (!_source)
            {
                return;
            }
            
            _processor = _processingFactory ? _processingFactory.Make() : null;

            IObservable<T> observable =
                _distinct ? _source.SignalObservable.DistinctUntilChanged() : _source.SignalObservable;

            if (_processor != null)
            {
                float startTime = Time.time;
                _subscription = observable.Subscribe(value =>
                {
                    Apply(_processor.Process(value, Time.time - startTime));
                });    
            }
            else
            {
                _subscription = observable.Subscribe(Apply); 
            }
        }

        protected virtual void OnDisable()
        {
            _processor = null;
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void Apply(T value);
    }
}