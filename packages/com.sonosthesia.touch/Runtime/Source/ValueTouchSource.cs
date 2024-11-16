using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Interaction;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ValueTouchSource<TValue> : TouchSource where TValue : struct
    {
        [SerializeField] private ChannelDriver<TValue> _driver;

        // we use composition with ValueTouchEndpoint so that affordances can apply to both sources and actors
        
        [SerializeField] private TouchValueEventChannel<TValue> _values;
        public TouchValueEventChannel<TValue> Values => _values;

        private readonly Dictionary<Guid, BehaviorSubject<ValueEvent<TValue, TouchEvent>>> _valueEventSubjects = new();

        protected override bool IsCompatibleActor(TouchActor actor) => actor is ValueTouchActor<TValue>;

        protected override bool ConfigureStream(Guid id, ITouchData touchData)
        {
            TouchEvent sourceEvent = new TouchEvent(touchData, Time.time);
            if (!Extract(true, sourceEvent.touchData, out TValue value))
            {
                return false;
            }

            if (_driver)
            {
                _driver.BeginStream(value, id);
            }
            
            BehaviorSubject<ValueEvent<TValue, TouchEvent>> subject = new (new ValueEvent<TValue, TouchEvent>(value, sourceEvent));
            _valueEventSubjects[id] = subject;

            IObservable<TouchEvent> eventObservable = subject.Select(_ => sourceEvent);
            IObservable<ValueEvent<TValue, TouchEvent>> valueObservable = subject.AsObservable();

            if (Node)
            {
                Node.Push(id, eventObservable);
            }
            if (Values)
            {
                Values.Push(id, valueObservable);    
            }
            
            // push the stream to the actor
            
            if (sourceEvent.touchData.Actor && sourceEvent.touchData.Actor.Node)
            {
                sourceEvent.touchData.Actor.Node.Push(id, eventObservable);
            }
            if (sourceEvent.touchData.Actor is ValueTouchActor<TValue> valueActor && valueActor.ValueEventChannel)
            {
                valueActor.ValueEventChannel.Push(id, valueObservable);
            }

            return true;
        }

        protected override void UpdateStream(Guid id, ITouchData touchData)
        {
            if (!Extract(false, touchData, out TValue value))
            {
                return;
            }

            if (_driver)
            {
                _driver.UpdateStream(id, value);    
            }

            if (!_valueEventSubjects.TryGetValue(id, out BehaviorSubject<ValueEvent<TValue, TouchEvent>> subject))
            {
                return;
            }

            ValueEvent<TValue, TouchEvent> current = subject.Value;
            subject.OnNext(new ValueEvent<TValue, TouchEvent>(value, current.Event));
        }

        protected override void CleanupStream(Guid id, ITouchData touchData)
        {
            if (_driver)
            {
                _driver.EndStream(id);    
            }

            if (!_valueEventSubjects.TryGetValue(id, out BehaviorSubject<ValueEvent<TValue, TouchEvent>> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();

            _valueEventSubjects.Remove(id);
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