using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    // Simpler version of ValueTriggerSource which does not drive a channel, and therefore with no associated value type
    // TODO: try to avoid code repetition but note that it is hard to do without introducing a big load of complexity

    public interface ITriggerSource
    {
        void KillStream(Guid id);

        void KillAllStreams();
    }
     
    public abstract class TriggerSource : TriggerEndpoint, ITriggerSource
    {
        [SerializeField] private bool _log;

        [SerializeField] private bool _endOnExit = true;

        [SerializeField] private bool _endOnReEnter = true;

        [SerializeField] private bool _autoEnd;

        [SerializeField] private float _autoEndDelay;

        // note : we don't want concurrent events from the same collider

        private readonly Dictionary<Collider, Guid> _triggerEvents = new();

        private readonly Dictionary<Guid, TriggerData> _triggerData = new();

        public void KillAllStreams()
        {
            Dictionary<Guid, TriggerData> copy = new Dictionary<Guid, TriggerData>(_triggerData);
            foreach (KeyValuePair<Guid, TriggerData> pair in copy)
            {
                EndStream(pair.Key, pair.Value);
            }
        }

        public void KillStream(Guid id)
        {
            if (_triggerData.TryGetValue(id, out TriggerData triggerData))
            {
                EndStream(id, triggerData);
            }
        }

        protected virtual bool IsCompatibleActor(TriggerActor actor) => true;

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
            
            TriggerActor actor = other.GetComponentInParent<TriggerActor>();

            if (!actor)
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (no actor)");
                return;
            }

            if (!IsCompatibleActor(actor))
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (incompatible actor)");
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
        
        private void BeginStream(TriggerData triggerData)
        {
            Guid eventId = Guid.NewGuid();
            
            _triggerEvents[triggerData.Collider] = eventId;
            _triggerData[eventId] = triggerData;

            if (!ConfigureStream(eventId, triggerData))
            {
                EndStream(eventId, triggerData);
                return;
            }

            if (_autoEnd)
            {
                Observable.Timer(TimeSpan.FromSeconds(_autoEndDelay)).Subscribe(_ => EndStream(eventId, triggerData));
            }
        }
        
        private void EndStream(Guid eventId, TriggerData triggerData)
        {
            _triggerEvents.Remove(triggerData.Collider);
            _triggerData.Remove(eventId);
            
            CleanupStream(eventId, triggerData);
        }

        protected abstract bool ConfigureStream(Guid eventId, ITriggerData triggerData);

        protected abstract void UpdateStream(Guid eventId, ITriggerData triggerData);

        protected abstract void CleanupStream(Guid eventId, ITriggerData triggerData);
    }
}