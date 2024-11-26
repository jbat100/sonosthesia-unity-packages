using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using VContainer;

namespace Sonosthesia.Application
{
    public class ApplicationStateActivator : MonoBehaviour
    {
        [SerializeField] private ApplicationStateSwitchSelector _selector;

        [SerializeField] private bool _invert;
        
        [SerializeField] private List<GameObject> _targets;

        private ApplicationState _state;
        
        [Inject]
        public void Construct(ApplicationState state)
        {
            _state = state;
        }

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();

            BoolReactiveProperty property = _state?.Select(_selector);

            if (property == null)
            {
                return;
            }

            _subscription = property.Subscribe(state =>
            {
                bool active = state ^ _invert;
                foreach (GameObject target in _targets)
                {
                    target.SafeSetActive(active);
                }
            });
        }

    }
}