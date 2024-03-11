using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
    public class ConfigurableGroupTransformer : GroupTransformer
    {
        [SerializeField] private GroupTransformerConfiguration _configuration;

        private IDisposable _changeSubscription;

        protected virtual void OnEnable()
        {
            _changeSubscription?.Dispose();
            _changeSubscription = _configuration.ChangeObservable.Subscribe(_ => BroadcastChange());
        }
        
        protected virtual void OnDisable()
        {
            _changeSubscription?.Dispose();
            _changeSubscription = null;
        }
        
        public override void Apply<T>(IEnumerable<T> targets)
        {
            if (!_configuration)
            {
                return;
            }
            
            _configuration.Apply(targets);
        }
    }
}