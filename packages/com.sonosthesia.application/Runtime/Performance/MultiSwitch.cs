using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Sonosthesia.Application
{
    public class MultiSwitch : ObservableBehaviour
    {
        [SerializeField] private MultiSwitchRegister _register;

        [SerializeField] private List<GameObject> _targets;
        public IReadOnlyList<GameObject> Targets => _targets;

        private readonly CompositeDisposable _targetSubscriptions = new();
        
        protected override void OnChanged()
        {
            base.OnChanged();
            _targetSubscriptions.Clear();
            foreach (GameObject target in _targets)
            {
                _targetSubscriptions.Add(
                    target.OnEnableAsObservable().Merge(target.OnDisableAsObservable())
                        .Subscribe(_ => BroadcastChange())
                    );
            }
        }

        protected virtual void OnEnable()
        {
            base.OnEnable();
            _register.Register(this);
        }

        protected virtual void OnDisable()
        {
            _targetSubscriptions.Clear();
            _register.Unregister(this);
        }
    }
}