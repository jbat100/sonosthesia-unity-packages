using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Flow
{
    public class SoftLandingOperator : Operator<float>
    {
        [SerializeField] private float _landingSpeed = 1;

        private SoftLanding _softLanding;
        
        protected override IDisposable Setup(Signal<float> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                _softLanding.Target = value;
            });
        }

        protected virtual void Update()
        {
            _softLanding.Step(Time.deltaTime);
            Broadcast(_softLanding.Current);
        }
    }
}