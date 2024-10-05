using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ValueTouchSource<TValue> : TouchSource, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> 
        where TValue : struct
    {
        [SerializeField] private ChannelDriver<TValue> _driver;

        // we use composition with ValueTouchEndpoint so that affordances can apply to both sources and actors
        
        [SerializeField] private ValueTouchEndpoint<TValue> _endpoint;

        private readonly Dictionary<Guid, BehaviorSubject<TriggerValueEvent<TValue>>> _valueEventSubjects = new();

        public StreamNode<TriggerValueEvent<TValue>> ValueStreamNode => _endpoint ? _endpoint.ValueStreamNode : null;

        protected override bool IsCompatibleActor(TouchActor actor) =>
            actor is IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>;

        protected override bool ConfigureStream(Guid eventId, ITouchData touchData)
        {
            TouchEvent sourceEvent = new TouchEvent(eventId, touchData, Time.time);
            if (!Extract(true, sourceEvent.TouchData, out TValue value))
            {
                return false;
            }

            if (_driver)
            {
                _driver.BeginStream(value, sourceEvent.Id);
            }
            
            BehaviorSubject<TriggerValueEvent<TValue>> subject = new BehaviorSubject<TriggerValueEvent<TValue>>(new TriggerValueEvent<TValue>(sourceEvent, value));
            _valueEventSubjects[sourceEvent.Id] = subject;

            IObservable<TouchEvent> sourceObservable = subject.Select(_ => sourceEvent);
            IObservable<TriggerValueEvent<TValue>> valueObservable = subject.AsObservable();
            
            EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            ValueStreamNode.Push(sourceEvent.Id, valueObservable);

            // push the stream to the actor
            if (sourceEvent.TouchData.Actor)
            {
                sourceEvent.TouchData.Actor.EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            }
            if (sourceEvent.TouchData.Actor is IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> actor)
            {
                actor.ValueStreamNode.Push(sourceEvent.Id, valueObservable);
            }

            return true;
        }

        protected override void UpdateStream(Guid eventId, ITouchData touchData)
        {
            if (!Extract(false, touchData, out TValue value))
            {
                return;
            }

            if (_driver)
            {
                _driver.UpdateStream(eventId, value);    
            }

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnNext(subject.Value.Update(value));
        }

        protected override void CleanupStream(Guid eventId, ITouchData touchData)
        {
            if (_driver)
            {
                _driver.EndStream(eventId);    
            }

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();

            _valueEventSubjects.Remove(eventId);
        }

        protected abstract bool Extract(bool initial, ITouchData touchData, out TValue value);
        
        public void TestStream(TValue value)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Stream test requires play mode");
                return;
            }

            if (!_driver)
            {
                Debug.LogError("TestStream requires driver");
                return;
            }
            Guid id = _driver.BeginStream(value);
            Observable.Timer(TimeSpan.FromSeconds(1f)).TakeUntilDestroy(this).Subscribe(_ => _driver.EndStream(id));
        }
    }
}