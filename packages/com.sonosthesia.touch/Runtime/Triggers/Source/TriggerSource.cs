using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerSource<TValue> : ValueTriggerEndpoint<TValue> where TValue : struct
    {
        [SerializeField] private bool _log;
        
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
            if (_log)
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} {other}");    
            }
            
            TriggerData triggerData;
            
            TriggerActor<TValue> actor = other.GetComponentInParent<TriggerActor<TValue>>();

            if (!actor)
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (no actor)");
                return;
            }

            // if gates fail we don't want to go ahead with stream _endOnReEnter
            if (!CheckGates(this, actor) || !actor.CheckGates(this, actor))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (gate block)");
                }
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
                        if (_log)
                        {
                            Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (existing stream)");
                        }
                        return;
                    }
                }
            }

            if (!actor.RequestPermission(other))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (actor refused permission)");
                }
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
            // Debug.Log($"{this} {nameof(OnTriggerStay)} {other.gameObject.name}");

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
            if (_log)
            {
                Debug.Log($"{this} {nameof(OnTriggerExit)} {other}");    
            }

            if (!_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerExit)} bailing out no event");    
                }
                return;
            }

            if (!_triggerData.TryGetValue(eventId, out TriggerData triggerData))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerExit)} bailing out no data");    
                }
                return;
            }

            triggerData.Colliding = false;

            if (_endOnExit)
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerExit)} ending stream {eventId}");    
                }
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
            
            Guid eventId = _driver ? _driver.BeginStream(value) : Guid.NewGuid();

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

        private void EndStream(Guid eventId, TriggerData triggerData)
        {
            if (_driver)
            {
                _driver.EndStream(eventId);    
            }
            
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

            if (!_driver)
            {
                Debug.LogError("TestStream requires driver");
                return;
            }
            Guid id = _driver.BeginStream(value);
            double delay = _autoEnd ? _autoEndDelay : 1f;
            Observable.Timer(TimeSpan.FromSeconds(delay)).TakeUntilDestroy(this).Subscribe(_ => _driver.EndStream(id));
        }
    }
}