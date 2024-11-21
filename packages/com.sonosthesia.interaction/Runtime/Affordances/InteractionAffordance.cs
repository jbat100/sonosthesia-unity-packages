using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sonosthesia.Interaction
{
    public class InteractionAffordance<TEvent> : MonoBehaviour, ILogSwitch where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private List<InteractionAffordanceGate> _gates;
        
        [FormerlySerializedAs("_streamContainers")] 
        [SerializeField] private List<Channel.Channel<TEvent>> _inputs;

        [SerializeField] private Channel.Channel<TEvent> _relay;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<TEvent> MakeController(Guid id) => null;
        
        protected virtual void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            
        }

        protected virtual bool CheckCompatibility(TEvent e) => true;

        // a bit of a pain to have to use async, but we need to wait for the first stream element to check
        private async UniTaskVoid OnStream(Guid id, IObservable<TEvent> stream)
        {
            stream = stream.TakeUntilDisable(this);

            TEvent e = await stream.ToUniTask(true);
            
            if (!CheckCompatibility(e))
            {
                return;
            }

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
                else if (gate is IInteractionAffordanceGate interactionGate)
                {
                    if (!interactionGate.Check(e))
                    {
                        this.LogVerbose($"{this} bailout on gate {e}");
                        return;    
                    }
                }
            }
            
            this.LogWarning($"{this} handling new stream {stream}");

            System.IObserver<TEvent> controller = MakeController(id);
            if (controller != null)
            {
                // Debug.LogWarning($"{this} created new controller {id}");
                stream.Subscribe(controller);
            }
            HandleStream(id, stream);

            if (_relay)
            {
                _relay.Push(id, stream);   
            }
        }

        protected virtual void OnEnable()
        {
            foreach (Channel.Channel<TEvent> channel in _inputs)
            {
                if (!channel)
                {
                    continue;
                }
                
                _subscriptions.Add(channel.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
                _subscriptions.Add(channel.Observable.Subscribe(pair =>
                {
                    this.LogVerbose($"{this} received new stream {pair.Key}");
                    OnStream(pair.Key, pair.Value).Forget();
                }));    
            }
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}