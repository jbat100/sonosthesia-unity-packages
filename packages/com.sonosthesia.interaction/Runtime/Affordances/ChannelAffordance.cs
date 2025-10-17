using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Channel;

namespace Sonosthesia.Interaction
{
    // Drives a ValueEvent<TValue, TEvent> channel from TEvent channel
    
    public abstract class ChannelAffordanceController<TValue, TEvent, TAffordance> : AffordanceController<TEvent, TAffordance> 
        where TValue : struct
        where TEvent : struct
        where TAffordance : ChannelAffordance<TValue, TEvent>
    {
        private bool _active;
        private BehaviorSubject<ValueEvent<TValue, TEvent>> _subject;
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

            ValueEvent<TValue, TEvent> valueEvent = new ValueEvent<TValue, TEvent>(_original, e);

            _subject = new BehaviorSubject<ValueEvent<TValue, TEvent>>(valueEvent);

            IObservable<ValueEvent<TValue, TEvent>> output = _subject.AsObservable();
            IObservable<TValue> values = output.Select(item => item.Value);

            if (Affordance.Output)
            {
                Affordance.Output.Push(EventId, output);
            }

            if (Affordance.Values)
            {
                Affordance.Values.Push(EventId, values);
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
                
                Channel<ValueEvent<TValue, TEvent>> endpointOutput = component.GetComponent<Channel<ValueEvent<TValue, TEvent>>>();
                if (endpointOutput)
                {
                    endpointOutput.Push(EventId, output);
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

            _subject.OnNext(new ValueEvent<TValue, TEvent>(updated, e));
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
        [SerializeField] private Channel<ValueEvent<TValue, TEvent>> _output;
        public Channel<ValueEvent<TValue, TEvent>> Output => _output;
        
        [SerializeField] private Channel<TValue> _values;
        public Channel<TValue> Values => _values;
    }
}