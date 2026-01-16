using System;
using System.Linq;
using Sonosthesia.Channel;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class AffordanceGroupInputs<TEvent> : MonoBehaviour where TEvent : struct
    {
        [SerializeField] private InterfaceReference<IObjectGroup> _group;

        [SerializeField] private InteractionAffordance<TEvent> _affordance;

        private IDisposable _subscription; 
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _subscription = _group.Value?.ObjectsChangedObservable.Subscribe(_ => Apply());
            Apply();
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void Apply()
        {
            if (!_affordance || !_group)
            {
                return;
            }
            _affordance.SetInputs(_group.Value.Objects.Select(o => o.GetComponent<IChannel<TEvent>>()));
        }
    }
}