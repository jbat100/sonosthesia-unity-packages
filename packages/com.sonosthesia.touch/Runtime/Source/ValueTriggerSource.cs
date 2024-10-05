using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ValueTriggerSource<TValue> : TriggerSource, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> 
        where TValue : struct
    {
        [SerializeField] private ChannelDriver<TValue> _driver;

        private readonly Dictionary<Guid, BehaviorSubject<TriggerValueEvent<TValue>>> _valueEventSubjects = new();
        
        private StreamNode<TriggerValueEvent<TValue>> _valueStreamNode;
        public StreamNode<TriggerValueEvent<TValue>> ValueStreamNode => _valueStreamNode ??= new StreamNode<TriggerValueEvent<TValue>>(this);

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _valueStreamNode?.Dispose();
        }
        
        protected override bool IsCompatibleActor(TriggerActor actor) =>
            actor is IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>;

        protected override bool ConfigureStream(Guid eventId, ITriggerData triggerData)
        {
            TriggerEvent sourceEvent = new TriggerEvent(eventId, triggerData, Time.time);
            if (!Extract(true, sourceEvent.TriggerData, out TValue value))
            {
                return false;
            }

            if (_driver)
            {
                _driver.BeginStream(value, sourceEvent.Id);
            }
            
            BehaviorSubject<TriggerValueEvent<TValue>> subject = new BehaviorSubject<TriggerValueEvent<TValue>>(new TriggerValueEvent<TValue>(sourceEvent, value));
            _valueEventSubjects[sourceEvent.Id] = subject;

            IObservable<TriggerEvent> sourceObservable = subject.Select(_ => sourceEvent);
            IObservable<TriggerValueEvent<TValue>> valueObservable = subject.AsObservable();
            
            EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            ValueStreamNode.Push(sourceEvent.Id, valueObservable);

            // push the stream to the actor
            if (sourceEvent.TriggerData.Actor)
            {
                sourceEvent.TriggerData.Actor.EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            }
            if (sourceEvent.TriggerData.Actor is IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> actor)
            {
                actor.ValueStreamNode.Push(sourceEvent.Id, valueObservable);
            }

            return true;
        }

        protected override void UpdateStream(Guid eventId, ITriggerData triggerData)
        {
            if (!Extract(false, triggerData, out TValue value))
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

        protected override void CleanupStream(Guid eventId, ITriggerData triggerData)
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

        protected abstract bool Extract(bool initial, ITriggerData triggerData, out TValue value);
        
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