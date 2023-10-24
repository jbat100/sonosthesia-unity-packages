using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class SignalLerper<T> : Signal<T> where T : struct
    {
        [SerializeField] private Signal<T> _source;

        [SerializeField] private float _lerp;

        private readonly struct Point
        {
            public readonly T Value;
            public readonly float Time;

            public Point(T value, float time)
            {
                Value = value;
                Time = time;
            }
        }
        
        private Point _reference;
        private Point _target;
        private bool _aligned;

        private IDisposable _subscription;
        
        protected abstract T Lerp(T current, T target, float lerp);

        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (!_source)
            {
                return;
            }
            _reference = _target = new Point(Value, Time.time);
            _subscription = _source.SignalObservable.Subscribe(value =>
            {
                _aligned = false;
                _reference = new Point(Value, Time.time);
                _target = new Point(value, Time.time + _lerp);
            });
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
        }
        

        protected void Update()
        {
            if (Time.time >= _target.Time && !_aligned)
            {
                _aligned = true;
                Broadcast(_target.Value);    
            }
            else
            {
                Broadcast(Lerp(_reference.Value, _target.Value, (Time.time - _reference.Time) / (_target.Time - _reference.Time)));  
            }
        }
    }
}