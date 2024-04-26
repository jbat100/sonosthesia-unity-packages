using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerSource<TValue> : ValueTriggerEndpoint<TValue> where TValue : struct
    {
        [SerializeField] private ChannelDriver<TValue> _driver;

        [SerializeField] private bool _endOnExit = true;

        [SerializeField] private bool _endOnReEnter = true;

        [SerializeField] private bool _autoEnd;

        [SerializeField] private float _autoEndDelay;

        private class TriggerData : ITriggerData
        {
            public Collider Collider { get; set; }
            public bool Colliding { get; set; }
            public TriggerEndpoint Source { get; set; }
            public TriggerEndpoint Actor { get; set; }
        }

        // note : we don't want concurrent events from the same collider

        private readonly Dictionary<Collider, Guid> _triggerEvents = new();

        private readonly Dictionary<Guid, TriggerData> _triggerData = new();

        private readonly Dictionary<Guid, BehaviorSubject<TriggerValueEvent<TValue>>> _valueEventSubjects = new();

        protected virtual void FixedUpdate()
        {
            foreach (KeyValuePair<Collider, Guid> pair in _triggerEvents)
            {
                if (!_triggerData.TryGetValue(pair.Value, out TriggerData triggerData) || triggerData.Colliding)
                {
                    continue;
                }
                UpdateStream(pair.Value, triggerData);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{this} {nameof(OnTriggerEnter)} {other}");

            TriggerData triggerData;
            
            TriggerActor<TValue> actor = other.GetComponentInParent<TriggerActor<TValue>>();
            
            // if gates fail we don't want to go ahead with stream _endOnReEnter
            if (!CheckGates(this, actor) || !actor || !actor.CheckGates(this, actor))
            {
                return;
            }
            
            if (_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                if (_triggerData.TryGetValue(eventId, out triggerData))
                {
                    if (_endOnReEnter)
                    {
                        EndStream(eventId, triggerData);
                    }
                    else
                    {
                        triggerData.Colliding = true;
                        UpdateStream(eventId, triggerData);
                        return;
                    }
                }
            }

            if (!actor.RequestPermission(other))
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

            BeginStream(triggerData);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            Debug.Log($"{this} {nameof(OnTriggerStay)} {other.gameObject.name}");

            if (!_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                return;
            }

            if (_triggerData.TryGetValue(eventId, out TriggerData triggerData))
            {
                UpdateStream(eventId, triggerData);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            Debug.Log($"{this} {nameof(OnTriggerExit)} {other}");

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
                EndStream(eventId, triggerData);
            }
        }

        public override void EndAllStreams()
        {
            Dictionary<Guid, TriggerData> copy = new Dictionary<Guid, TriggerData>(_triggerData);
            foreach (KeyValuePair<Guid, TriggerData> pair in copy)
            {
                EndStream(pair.Key, pair.Value);
            }
        }

        public override void EndStream(Guid id)
        {
            if (_triggerData.TryGetValue(id, out TriggerData triggerData))
            {
                EndStream(id, triggerData);
            }
        }

        private void BeginStream(TriggerData triggerData)
        {
            if (!Extract(true, triggerData, out TValue value))
            {
                return;
            }
            
            Guid eventId = _driver.BeginStream(value);

            _triggerEvents[triggerData.Collider] = eventId;
            _triggerData[eventId] = triggerData;

            float startTime = Time.time;
            
            BehaviorSubject<TriggerValueEvent<TValue>> subject = new BehaviorSubject<TriggerValueEvent<TValue>>(new TriggerValueEvent<TValue>(eventId, triggerData, value, startTime));
            _valueEventSubjects[eventId] = subject;

            TriggerEvent sourceEvent = new TriggerEvent(eventId, triggerData, startTime);

            IObservable<TriggerEvent> sourceObservable = subject.Select(_ => sourceEvent);
            IObservable<TriggerValueEvent<TValue>> valueObservable = subject.AsObservable();
            
            EventStreamNode.Push(eventId, sourceObservable);
            ValueStreamNode.Push(eventId, valueObservable);

            // push the stream to the actor
            if (triggerData.Actor)
            {
                triggerData.Actor.EventStreamNode.Push(eventId, sourceObservable);
            }
            if (triggerData.Actor is TriggerActor<TValue> actor)
            {
                actor.ValueStreamNode.Push(eventId, valueObservable);
            }

            if (AutoEnd(value, out float delay))
            {
                Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ => sourceEvent.EndStream());
            }
            
        }

        private void UpdateStream(Guid eventId, TriggerData triggerData)
        {
            if (!Extract(false, triggerData, out TValue value))
            {
                return;
            }
            
            _driver.UpdateStream(eventId, value);

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnNext(subject.Value.Update(value));
        }

        private void EndStream(Guid eventId, TriggerData triggerData)
        {
            _driver.EndStream(eventId);
            
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

        protected virtual bool AutoEnd(TValue initial, out float delay)
        {
            delay = _autoEndDelay;
            return _autoEnd;
        }

        public void TestStream(TValue value)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Stream test requires play mode");
                return;
            }
            Guid id = _driver.BeginStream(value);
            double delay = _autoEnd ? _autoEndDelay : 1f;
            Observable.Timer(TimeSpan.FromSeconds(delay)).TakeUntilDestroy(this).Subscribe(_ => _driver.EndStream(id));
        }
    }
}