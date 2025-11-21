using System;
using Sonosthesia.Processing;
using Sonosthesia.Utils;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Signal
{
    public abstract class Target<TValue, TProcessor> : MonoBehaviour, ILogSwitch 
        where TValue : struct where TProcessor : IProcessor<TValue>
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        [SerializeField] private Signal<TValue> _source;

        [SerializeField] private bool _distinct = true; 

        [SerializeField] private DynamicProcessorFactory<TValue> _processingFactory;
        
        [SerializeField] private TProcessor _processor;

        private IDisposable _subscription;
        private IDynamicProcessor<TValue> _dynamicProcessor;

        protected virtual void Awake()
        {
            if (!_source)
            {
                _source = GetComponent<Signal<TValue>>();
            }
        }
        
        protected virtual void OnEnable() 
        {
            _subscription?.Dispose();
            _dynamicProcessor = null;
            
            if (!_source)
            {
                return;
            }
            
            _dynamicProcessor = _processingFactory ? _processingFactory.Make() : null;

            IObservable<TValue> observable =
                _distinct ? _source.Observable.DistinctUntilChanged() : _source.Observable;

            if (_dynamicProcessor != null)
            {
                float startTime = Time.time;
                _subscription = observable.Subscribe(value =>
                {
                    TValue processed = _processor.Process(_dynamicProcessor.Process(value, Time.time - startTime)); 
                    this.LogVerbose($"{this} {nameof(Apply)} {processed}");
                    Apply(processed);
                });    
            }
            else
            {
                _subscription = observable.Subscribe(value =>
                {
                    this.LogVerbose($"{this} {nameof(Apply)} {value}");
                    Apply(_processor.Process(value));
                }); 
            }
        }

        protected virtual void OnDisable()
        {
            _dynamicProcessor = null;
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void Apply(TValue value);
    }

    public abstract class Target<TValue> : Target<TValue, PassthroughProcessor<TValue>> where TValue : struct
    {
        
    }
}