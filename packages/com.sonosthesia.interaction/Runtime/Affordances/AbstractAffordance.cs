using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class AbstractAffordance<TEvent> : MonoBehaviour, ILogSwitch where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private List<AbstractAffordanceGate> _gates;
        
        [SerializeField] private List<StreamContainer<TEvent>> _streamContainers;

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

            foreach (AbstractAffordanceGate gate in _gates)
            {
                if (gate is IAffordanceGate<TEvent> typedGate)
                {
                    if (!typedGate.Check(e))
                    {
                        this.LogVerbose($"{this} bailout on typed gate");
                        return;
                    }
                }
                else if (gate is IAffordanceGate interactionGate)
                {
                    if (!interactionGate.Check(e))
                    {
                        this.LogVerbose($"{this} bailout on gate");
                        return;    
                    }
                }
            }
            
            this.LogWarning($"{this} handling new stream {stream}");

            // TODO: check what happens in the case of controllers which live beyond the stream
            System.IObserver<TEvent> controller = MakeController(id);
            if (controller != null)
            {
                // Debug.LogWarning($"{this} created new controller {id}");
                stream.Subscribe(controller);
            }
            HandleStream(id, stream);
        }

        protected virtual void OnEnable()
        {
            foreach (StreamContainer<TEvent> streamContainer in _streamContainers)
            {
                if (!streamContainer)
                {
                    continue;
                }
                
                _subscriptions.Add(streamContainer.StreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
                _subscriptions.Add(streamContainer.StreamNode.StreamObservable.Subscribe(pair =>
                {
                    this.LogVerbose($"{this} received new stream {pair.Key}");
                    OnStream(pair.Key, pair.Value).Forget();
                }));    
            }
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}