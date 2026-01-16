using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Channel;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class ChannelCountAffordance : MonoBehaviour
    {
        [SerializeField] 
        private List<InterfaceReference<IChannel>> _inputs;

        private CompositeDisposable _subscriptions = new ();
        
        protected void OnEnable()
        {
            _subscriptions.Clear();
            foreach (AbstractChannel channel in _inputs)
            {
                channel.Ids.ObserveCountChanged().Subscribe(_ => UpdateCount());
            }
            UpdateCount();
        }

        protected void OnDisable()
        {
            _subscriptions.Clear();
        }

        private void UpdateCount() => OnCountUpdated(_inputs.Select(i => i.Value?.Ids.Count ?? 0).Sum());
        
        protected abstract void OnCountUpdated(int count);
    }
}