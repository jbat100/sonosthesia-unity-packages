using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class WarpFloatSignalOperator : Operator<float>
    {
        [SerializeField] private float _offset;
        
        [SerializeField] private float _scale = 1f;

        protected override IDisposable Setup(Signal<float> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                float eased = Mathf.Lerp(0f, 1f, value);
                Broadcast(eased * _scale + _offset);
            });
        }
    }
}