using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Interaction
{
    public class AbstractAffordance<TEvent> : MonoBehaviour where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private InteractionLayerMask _sourceLayers;
        public InteractionLayerMask SourceLayers => _sourceLayers;
        
        [SerializeField] private InteractionLayerMatch _sourceMatch = InteractionLayerMatch.Any;
        public InteractionLayerMatch SourceMatch => _sourceMatch;
        
        [SerializeField] private InteractionLayerMask _actorLayers;
        public InteractionLayerMask ActorLayers => _actorLayers;
        
        [SerializeField] private InteractionLayerMatch _actorMatch = InteractionLayerMatch.Any;
        public InteractionLayerMatch ActorMatch => _actorMatch;

        [SerializeField] private List<StreamContainer<TEvent>> _streamContainers;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<TEvent> MakeController(Guid id) => null;
        
        protected virtual void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            
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
                    Guid id = pair.Key;
                    if (Log)
                    {
                        Debug.Log($"{this} handling new stream {pair.Key}");
                    }
                    IObservable<TEvent> stream = pair.Value.TakeUntilDisable(this);
                    
                    // TODO: check what happens in the case of controllers which live beyond the stream
                    System.IObserver<TEvent> controller = MakeController(id);
                    if (controller != null)
                    {
                        Debug.LogWarning($"{this} created new controller {id}");
                        stream.Subscribe(controller);
                    }
                    HandleStream(id, stream);
                }));    
            }
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}