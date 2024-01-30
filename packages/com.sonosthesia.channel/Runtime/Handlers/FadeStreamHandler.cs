using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Flow;
using Sonosthesia.Utils;

namespace Sonosthesia.Channel
{
    public abstract class FadeStreamHandler<T> : StreamHandler<T> where T : struct
    {
        [SerializeField] private Selector<T> _selector;
        
        [SerializeField] private Signal<float> _destination;
        
        [SerializeField] private AnimationCurve _fadeIn;
        
        [SerializeField] private AnimationCurve _fadeOut;
        
        private float? _firstTime;
        private float? _completionTime;
        private float _currentValue;
        
        protected virtual void Update()
        {
            if (_completionTime != null)
            {
                float fadeDuration = _fadeOut.Duration();
                float fadeTime = Time.time - _completionTime.Value;
                if (fadeTime < fadeDuration)
                {
                    _destination.Broadcast(_currentValue * _fadeOut.Evaluate(fadeTime));
                }
                else
                {
                    Complete();
                }
            }
            else if (_firstTime != null)
            {
                float fadeDuration = _fadeIn.Duration();
                float fadeTime = Time.time - _firstTime.Value;
                if (fadeTime < fadeDuration)
                {
                    _destination.Broadcast(_currentValue * _fadeIn.Evaluate(fadeTime)); 
                }
                else
                {
                    _destination.Broadcast(_currentValue);    
                }
            }
        }
        
        protected override IDisposable InternalHandleStream(IObservable<T> stream)
        {
            IDisposable startSubscription = stream.First().Subscribe(value =>
            {
                _firstTime = Time.time;
                _currentValue = _selector.Select(value);
            });
            IDisposable mainSubscription = stream.Subscribe(value =>
            {
                _currentValue = _selector.Select(value);
            }, error =>
            {
                Debug.LogError($"{this} {error.Message}");
                _completionTime = Time.time;
            } , () =>
            {
                _completionTime = Time.time;
            });
            return new CompositeDisposable {startSubscription, mainSubscription};
        }

        protected override void Complete()
        {
            base.Complete();
            _firstTime = null;
            _completionTime = null;
        }
    }
}