using System;
using System.Collections.Generic;
using Sonosthesia.Interaction;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchSource : TouchEndpoint
    {
        private class TouchData : ITouchData
        {
            public Collider Collider { get; set; }
            public bool Colliding { get; set; }
            public TouchSource Source { get; set; }
            public TouchActor Actor { get; set; }
        }
        
        [SerializeField] private bool _log;

        [SerializeField] private bool _endOnExit = true;

        [SerializeField] private bool _endOnReEnter = true;

        [SerializeField] private bool _autoEnd;

        [SerializeField] private float _autoEndDelay;

        // note : we don't want concurrent events from the same collider

        private readonly Dictionary<Collider, Guid> _triggerEvents = new();

        private readonly Dictionary<Guid, TouchData> _triggerData = new();

        public void KillAllStreams()
        {
            Dictionary<Guid, TouchData> copy = new Dictionary<Guid, TouchData>(_triggerData);
            foreach (KeyValuePair<Guid, TouchData> pair in copy)
            {
                EndStream(pair.Key, pair.Value);
            }
        }

        public void KillStream(Guid id)
        {
            if (_triggerData.TryGetValue(id, out TouchData triggerData))
            {
                EndStream(id, triggerData);
            }
        }

        protected virtual bool IsCompatibleActor(TouchActor actor) => true;

        protected virtual void FixedUpdate()
        {
            foreach (KeyValuePair<Collider, Guid> pair in _triggerEvents)
            {
                if (!_triggerData.TryGetValue(pair.Value, out TouchData triggerData) || triggerData.Colliding)
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
            
            TouchData touchData;
            
            TouchActor actor = other.GetComponentInParent<TouchActor>();

            if (!actor)
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (no actor)");
                return;
            }
            
            // bail out if the actor is already active through another collider

            foreach (TouchData data in _triggerData.Values)
            {
                if (data.Actor == actor)
                {
                    return;
                }
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
                if (_triggerData.TryGetValue(eventId, out touchData))
                {
                    if (_endOnReEnter)
                    {
                        EndStream(eventId, touchData);
                    }
                    else
                    {
                        touchData.Colliding = true;
                        UpdateStream(eventId, touchData);
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
            
            touchData = new TouchData()
            {
                Collider = other,
                Colliding = true,
                Actor = actor,
                Source = this
            };

            BeginStream(touchData);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            // Debug.Log($"{this} {nameof(OnTriggerStay)} {other.gameObject.name}");

            if (!_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                return;
            }

            if (_triggerData.TryGetValue(eventId, out TouchData triggerData))
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

            if (!_triggerData.TryGetValue(eventId, out TouchData triggerData))
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
        
        private void BeginStream(TouchData touchData)
        {
            Guid id = Guid.NewGuid();
            
            _triggerEvents[touchData.Collider] = id;
            _triggerData[id] = touchData;

            if (!ConfigureStream(id, touchData))
            {
                EndStream(id, touchData);
                return;
            }

            if (_autoEnd)
            {
                Observable.Timer(TimeSpan.FromSeconds(_autoEndDelay)).Subscribe(_ => EndStream(id, touchData));
            }
        }
        
        private void EndStream(Guid id, TouchData touchData)
        {
            _triggerEvents.Remove(touchData.Collider);
            _triggerData.Remove(id);
            
            CleanupStream(id, touchData);
        }

        protected abstract bool ConfigureStream(Guid id, ITouchData touchData);

        protected abstract void UpdateStream(Guid id, ITouchData touchData);

        protected abstract void CleanupStream(Guid id, ITouchData touchData);
    }
}