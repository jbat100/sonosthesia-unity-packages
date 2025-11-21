using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Utils;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public class SoftLandingOperator : Operator<float>
    {
        [SerializeField] private float _speed = 1;

        private SoftLanding _softLanding;
        
        protected override IDisposable Setup(Signal<float> source)
        {
            return source.Observable.Subscribe(value =>
            {
                _softLanding.Target = value;
            });
        }

        protected virtual void Update()
        {
            _softLanding.Speed = _speed;
            _softLanding.Step(Time.deltaTime);
            Broadcast(_softLanding.Current);
        }
    }
}