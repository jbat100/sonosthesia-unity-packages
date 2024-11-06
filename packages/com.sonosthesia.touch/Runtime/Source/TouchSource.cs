using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Interaction;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchSource : TouchEndpoint
    {
        protected class TouchData : ITouchData
        {
            public Guid Id;
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

        [SerializeField] private InteractionLayerMatch _actorMatch = InteractionLayerMatch.Any;

        // note : we don't want concurrent events from the same collider
        
        private readonly Dictionary<TouchActor, TouchData> _touchData = new();
        private readonly HashSet<TouchActor> _gatedActors = new();

        public void KillAllStreams()
        {
            foreach (TouchData data in _touchData.Values.ToArray())
            {
                EndStream(data);
            }
        }

        public void KillStream(Guid id)
        {
            foreach (TouchData data in _touchData.Values.Where(d => d.Id == id).ToArray())
            {
                EndStream(data);
            }
        }

        protected virtual bool IsCompatibleActor(TouchActor actor) => true;

        protected virtual void FixedUpdate()
        {
            foreach (var data in _touchData.Values.Where(data => !data.Colliding))
            {
                UpdateStream(pair.Value, triggerData);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_log)
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} {other}");    
            }

            TouchActor actor = other.GetComponentInParent<TouchActor>();

            if (!actor)
            {
                Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (no actor)");
                return;
            }
            
            // bail out if the actor is already active through this or another collider

            if (_currentActors.Contains(actor))
            {
                if (!_streamIds.TryGetValue(other, out Guid eventId))
                {
                    _
                }
                
                if (_streamIds.TryGetValue(other, out Guid eventId) && _touchData.TryGetValue(eventId, out TouchData touchData))
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

            // check interaction layers
            
            if (!_actorMatch.Match(InteractionLayers, actor.InteractionLayers))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (no match)");
                }
                return;
            }

            if (!IsCompatibleActor(actor))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (incompatible actor)");
                }
                return;
            }

            if (Mute || actor.Mute)
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (gate block)");
                }
                return;
            }

            if (!CheckGates(actor))
            {
                
            }

            AttemptStream(actor, other);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            // Debug.Log($"{this} {nameof(OnTriggerStay)} {other.gameObject.name}");

            if (!_streamIds.TryGetValue(other, out Guid eventId))
            {
                return;
            }

            if (_touchData.TryGetValue(eventId, out TouchData triggerData))
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

            if (!_streamIds.TryGetValue(other, out Guid eventId))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerExit)} bailing out no event");    
                }
                return;
            }

            if (!_touchData.TryGetValue(eventId, out TouchData triggerData))
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

        private bool CheckGates(TouchActor actor)
        {
            if (!Gates.All(gate => gate.Check(this, actor)))
            {
                return false;
            }
            
            if (!actor.Gates.All(gate => gate.Check(this, actor)))
            {
                return false;
            }

            return true;
        }
        
        private bool AttemptStream(TouchActor actor, Collider other)
        {
            if (!actor.RequestPermission(other))
            {
                if (_log)
                {
                    Debug.Log($"{this} {nameof(OnTriggerEnter)} bailed out (actor refused permission)");
                }
                return false;
            }
            
            TouchData touchData = new TouchData()
            {
                Collider = other,
                Colliding = true,
                Actor = actor,
                Source = this
            };

            BeginStream(touchData);

            return true;
        }
        
        private void BeginStream(TouchData touchData)
        {
            Guid id = Guid.NewGuid();
            
            _streamIds[touchData.Collider] = id;
            _touchData[id] = touchData;

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
        
        private void EndStream(TouchData touchData)
        {
            _streamIds.Remove(touchData.Collider);
            _touchData.Remove(id);
            
            CleanupStream(id, touchData);
        }

        protected abstract bool ConfigureStream(Guid id, ITouchData touchData);

        protected abstract void UpdateStream(Guid id, ITouchData touchData);

        protected abstract void CleanupStream(Guid id, ITouchData touchData);
    }
}