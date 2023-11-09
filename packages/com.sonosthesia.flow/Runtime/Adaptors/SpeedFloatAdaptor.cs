using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class SpeedFloatAdaptor<TTarget> : Adaptor<float, TTarget> where TTarget : struct
    {
        [SerializeField] private float _start = 0f;
        
        [SerializeField] private float _offset = 0f;
        
        [SerializeField] private float _scale = 1f;
        
        [SerializeField] private float _range = 1f;

        protected abstract TTarget Map(float value);

        protected override IDisposable Setup(Signal<float> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                float eased = Mathf.Lerp(0f, 1f, value);
                _speed = eased * _scale + _offset;
            });
        }
        
        private float _speed;
        private float _value;

        protected override void OnEnable()
        {
            _speed = 0f;
            _value = _start;
            base.OnEnable();
        }

        protected void Update()
        {
            _value += _speed * Time.deltaTime;
            Broadcast(Map(_value % _range));
        }
    }
}