using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Channel;

namespace Sonosthesia.Interaction
{
    public abstract class ChannelAffordanceController<TValue, TEvent, TAffordance> : AffordanceController<TEvent, TAffordance> 
        where TValue : struct
        where TEvent : struct
        where TAffordance : ChannelAffordance<TValue, TEvent>
    {
        private bool _active;
        private BehaviorSubject<TValue> _subject;
        private TValue _original;
        
        public ChannelAffordanceController(Guid eventId, TAffordance affordance) : base(eventId, affordance)
        {
        }

        protected abstract bool Setup(TEvent e, out TValue original);

        protected abstract bool Update(TEvent e, TValue original, out TValue updated);
        
        protected sealed override void Setup(TEvent e)
        {
            base.Setup(e);
            
            _active = Setup(e, out _original);
            
            if (!_active)
            {
                return;
            }

            _subject = new BehaviorSubject<TValue>(_original);

            IObservable<TValue> values = _subject.AsObservable();

            if (Affordance.Output)
            {
                Affordance.Output.Push(EventId, values);
            }

            void PushToEndpoint(IInteractionEndpoint endpoint)
            {
                if (endpoint is not Component component)
                {
                    return;
                }

                Channel<TValue> endpointValues = component.GetComponent<Channel<TValue>>();
                if (endpointValues)
                {
                    endpointValues.Push(EventId, values);
                }
            }

            if (e is IInteractionEvent ie)
            {
                PushToEndpoint(ie.Actor);
                PushToEndpoint(ie.Source);   
            }
        }

        protected sealed override void Update(TEvent e)
        {
            base.Update(e);

            if (!_active || _subject == null || !Update(e, _original, out TValue updated))
            {
                return;
            }

            _subject.OnNext(updated);
        }

        protected sealed override void Teardown(TEvent e)
        {
            base.Teardown(e);
            _subject?.OnCompleted();
            _subject?.Dispose();
            _subject = null;
        }
    }

    public class ChannelAffordance<TValue, TEvent> : InteractionAffordance<TEvent>
        where TValue : struct
        where TEvent : struct
    {
        [SerializeField] private Channel<TValue> _output;
        public Channel<TValue> Output => _output;
    }
}