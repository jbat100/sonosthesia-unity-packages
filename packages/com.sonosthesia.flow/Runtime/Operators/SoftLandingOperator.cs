using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class SoftLandingOperator : Operator<float>
    {
        [SerializeField] private float _landingSpeed = 1;
        
        private float? _target;
        private float? _current;
        
        protected override IDisposable Setup(Signal<float> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                _target = value;
            });
        }

        protected virtual void Update()
        {
            _current ??= _target;
            if (!_current.HasValue || !_target.HasValue)
            {
                return;
            }
            _current = _current.Value > _target.Value ? 
                Mathf.MoveTowards(_current.Value, _target.Value, Time.deltaTime * _landingSpeed) : 
                _target;
            Broadcast(_current.Value);
        }
    }
}