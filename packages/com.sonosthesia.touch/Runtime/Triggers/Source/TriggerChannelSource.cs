using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public readonly struct TriggerValueEvent<TValue> where TValue : struct
    {
        public readonly Guid Id;
        public readonly ITriggerData TriggerData;
        public readonly TValue Value;

        public TriggerValueEvent(Guid id, ITriggerData triggerData, TValue value)
        {
            Id = id;
            TriggerData = triggerData;
            Value = value;
        }
    }

    public abstract class TriggerChannelSource<TValue> : BaseTriggerChannelSource where TValue : struct
    {
        [SerializeField] private ChannelDriver<TValue> _driver;

        [SerializeField] private bool _endOnExit = true;

        [SerializeField] private bool _restartOnEnter = true;

        private class TriggerData : ITriggerData
        {
            public Collider Collider { get; set; }
            public bool Colliding { get; set; }
            public BaseTriggerChannelSource Source { get; set; }
            public BaseTriggerActor Actor { get; set; }
        }

        // note : we don't want concurrent events from the same collider

        private readonly Dictionary<Collider, Guid> _triggerEvents = new();

        private readonly Dictionary<Guid, TriggerData> _triggerData = new();

        private readonly Dictionary<Guid, BehaviorSubject<TriggerValueEvent<TValue>>> _valueEventSubjects = new();

        private readonly Subject<IObservable<TriggerValueEvent<TValue>>> _valueStreamSubject = new();
        public IObservable<IObservable<TriggerValueEvent<TValue>>> ValueStreamObservable => _valueStreamSubject.AsObservable();

        protected virtual void FixedUpdate()
        {
            foreach (KeyValuePair<Collider, Guid> pair in _triggerEvents)
            {
                if (!_triggerData.TryGetValue(pair.Value, out TriggerData triggerData) || triggerData.Colliding)
                {
                    continue;
                }
                UpdateEvent(pair.Value, triggerData);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            //Debug.Log($"{this} {nameof(OnTriggerEnter)} {other}");

            TriggerData triggerData;

            if (_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                if (_triggerData.TryGetValue(eventId, out triggerData))
                {
                    if (_restartOnEnter)
                    {
                        EndEvent(eventId, triggerData);
                    }
                    else
                    {
                        triggerData.Colliding = true;
                        UpdateEvent(eventId, triggerData);
                        return;
                    }
                }
            }

            TriggerActor<TValue> actor = other.GetComponentInParent<TriggerActor<TValue>>();
            if (!actor || !actor.IsAvailable(other))
            {
                return;
            }

            triggerData = new TriggerData()
            {
                Collider = other,
                Colliding = true,
                Actor = actor,
                Source = this
            };

            BeginEvent(triggerData);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            //Debug.Log($"{this} {nameof(OnTriggerStay)} {other}");

            if (!_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                return;
            }

            if (_triggerData.TryGetValue(eventId, out TriggerData triggerData))
            {
                UpdateEvent(eventId, triggerData);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            //Debug.Log($"{this} {nameof(OnTriggerExit)} {other}");

            if (!_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                return;
            }

            if (!_triggerData.TryGetValue(eventId, out TriggerData triggerData))
            {
                return;
            }

            triggerData.Colliding = false;

            if (!_endOnExit)
            {
                EndEvent(eventId, triggerData);
            }
        }

        private void BeginEvent(TriggerData triggerData)
        {
            if (!Extract(true, triggerData, out TValue value))
            {
                return;
            }
            
            Guid eventId = _driver.BeginEvent(value);

            _triggerEvents[triggerData.Collider] = eventId;
            _triggerData[eventId] = triggerData;

            BehaviorSubject<TriggerValueEvent<TValue>> subject = new BehaviorSubject<TriggerValueEvent<TValue>>(new TriggerValueEvent<TValue>(eventId, triggerData, value));
            _valueEventSubjects[eventId] = subject;
            
            RegisterEvent(eventId);
            
            Pipe(subject.Select(valueEvent => new TriggerSourceEvent(eventId, triggerData)));
            
        }

        private void UpdateEvent(Guid eventId, TriggerData triggerData)
        {
            if (!Extract(false, triggerData, out TValue value))
            {
                return;
            }
            
            _driver.UpdateEvent(eventId, value);

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnNext(new TriggerValueEvent<TValue>(eventId, triggerData, value));
        }

        private void EndEvent(Guid eventId, TriggerData triggerData)
        {
            _driver.EndEvent(eventId);
            
            UnregisterEvent(eventId);

            _triggerEvents.Remove(triggerData.Collider);
            _triggerData.Remove(eventId);
            
            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();

            _valueEventSubjects.Remove(eventId);
        }

        protected abstract bool Extract(bool initial, ITriggerData triggerData, out TValue value);

        protected virtual void Clean(ITriggerData triggerData)
        {
            
        }
    }
}