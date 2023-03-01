using System;
using System.Collections.Generic;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Spawn
{
    [Serializable]
    public class VFXTargetGroupLink<T> where T : struct 
    {
        [SerializeField] private string _source;
        public string Source => _source;
            
        [SerializeField] private string _target;
        public string Target => _target;

        public T Map(T source) => InternalMap(source);

        protected virtual T InternalMap(T source) => source;
    }
    
    public abstract class VFXTargetGroup<T, TLink> : MonoBehaviour where T : struct where TLink : VFXTargetGroupLink<T>
    {
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] private SignalGroup<T> _signalGroup;

        [SerializeField] private List<TLink> _links = new ();

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            foreach (TLink link in _links)
            {
                Signal<T> signal = _signalGroup.GetSignal(link.Source);
                _subscriptions.Add(signal.SignalObservable.Subscribe(value =>
                {
                    T mapped = link.Map(value);
                    Apply(_visualEffect, link.Target, mapped);
                }));
            }
        }

        protected void OnDisable() => _subscriptions.Clear();

        protected abstract void Apply(VisualEffect visualEffect, string key, T value);
    }
}


