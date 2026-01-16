using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sonosthesia.Utils;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class InteractionAffordance<TEvent> : MonoBehaviour, ILogSwitch where TEvent : struct
    {
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] 
        private List<InteractionAffordanceGate> _gates;
        
        [SerializeField] 
        private List<InterfaceReference<IChannel<TEvent>>> _inputs;
        
        [SerializeField] 
        private InterfaceReference<IChannel<TEvent>> _relay;

        private readonly CompositeDisposable _subscriptions = new();

        public void SetInputs(IEnumerable<IChannel<TEvent>> inputs)
        {
            if (_inputs == null)
            {
                _inputs = new List<InterfaceReference<IChannel<TEvent>>>(inputs.Select(i => new InterfaceReference<IChannel<TEvent>>(i)));
            }
            else
            {
                _inputs.Clear();
                _inputs.AddRange(inputs.Select(i => new InterfaceReference<IChannel<TEvent>>(i)));
            }
            ReloadSubscriptions();
        }
        
        protected virtual void OnStartedStream(Guid id, TEvent e) { }
        
        protected virtual IObserver<TEvent> MakeController(Guid id) => null;

        // a bit of a pain to have to use async, but we need to wait for the first stream element to check
        private async UniTaskVoid OnStream(Guid id, IObservable<TEvent> stream)
        {
            stream = stream.TakeUntilDisable(this);

            TEvent e = await stream.ToUniTask(true);

            foreach (InteractionAffordanceGate gate in _gates)
            {
                this.LogVerbose($"{this} checking {e} on {gate}");
                if (gate is IInteractionAffordanceGate<TEvent> typedGate)
                {
                    if (!typedGate.Check(e))
                    {
                        this.LogVerbose($"{this} bailout on typed gate {e}");
                        return;
                    }
                }
                else if (gate is IInteractionAffordanceGate interactionGate && e is IInteractionEvent ie)
                {
                    if (!interactionGate.Check(ie))
                    {
                        this.LogVerbose($"{this} bailout on gate {e}");
                        return;    
                    }
                }
            }
            
            this.LogWarning($"{this} handling new stream {stream}");

            IObserver<TEvent> controller = MakeController(id);
            if (controller != null)
            {
                // Debug.LogWarning($"{this} created new controller {id}");
                stream.Subscribe(controller);
            }
            
            OnStartedStream(id, e);

            _relay.Value?.Push(id, stream);   
        }

        protected virtual void OnEnable()
        {
            ReloadSubscriptions();
        }

        protected virtual void OnDisable() => _subscriptions.Clear();

        private void ReloadSubscriptions()
        {
            _subscriptions.Clear();
            foreach (InterfaceReference<IChannel<TEvent>> channel in _inputs.Where(channel => channel))
            {
                _subscriptions.Add(channel.Value.Observable.Subscribe(pair =>
                {
                    this.LogVerbose($"{this} received new stream {pair.Key}");
                    OnStream(pair.Key, pair.Value).Forget();
                }));
            }
        }
    }
}