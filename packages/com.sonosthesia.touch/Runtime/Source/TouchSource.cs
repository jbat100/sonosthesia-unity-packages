using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchSource : TouchEndpoint
    {
        private class TouchData : ITouchData
        {
            public Guid Id;
            public Collider Collider { get; set; }
            public bool Colliding { get; set; }
            public TouchSource Source { get; set; }
            public TouchActor Actor { get; set; }
        }

        [SerializeField] private bool _endOnGates = true;
        [SerializeField] private bool _endOnExit = true;
        [SerializeField] private bool _endOnReEnter = true;
        [SerializeField] private bool _autoEnd;
        [SerializeField] private float _autoEndDelay;

        [SerializeField] private InteractionLayerMatch _actorMatch = InteractionLayerMatch.Any;
        
        // note : we don't want concurrent events from the same collider
        
        private readonly Dictionary<TouchActor, TouchData> _touchData = new();
        private readonly Dictionary<TouchActor, Collider> _gatedActors = new();

        // avoid alloc
        
        private static readonly List<TouchData> _reusableData = new();
        private static readonly List<TouchActor> _reusableActors = new();

        public void KillAllStreams()
        {
            foreach (TouchData data in _reusableData.Import(_touchData.Values))
            {
                EndStream(data);
            }
        }

        public void KillStream(Guid id)
        {
            foreach (TouchData data in _reusableData.Import(_touchData.Values.Where(d => d.Id == id)))
            {
                EndStream(data);
            }
        }

        protected virtual bool IsCompatibleActor(TouchActor actor) => true;

        // using FixedUpdate instead of OnTriggerStay because stream can stay active after collision ends
        
        protected virtual void FixedUpdate()
        {
            foreach (TouchActor actor in _reusableActors.Import(_gatedActors.Keys).Where(actor => !actor.isActiveAndEnabled))
            {
                _gatedActors.Remove(actor);
            }

            foreach (TouchData data in _reusableData.Import(_touchData.Values))
            {
                if (!data.Actor.isActiveAndEnabled)
                {
                    EndStream(data);
                    continue;
                }
                if (_endOnGates && !CheckGates(data.Actor))
                {
                    if (!CheckGates(data.Actor))
                    {
                        if (data.Colliding)
                        {
                            _gatedActors[data.Actor] = data.Collider;
                        }
                        EndStream(data);
                        continue;
                    }
                }
                UpdateStream(data.Id, data);
            }
        }

        protected virtual void OnDisable()
        {
            _gatedActors.Clear();
            KillAllStreams();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            this.LogVerbose($"{this} {nameof(OnTriggerEnter)} {other}");

            TouchActor actor = other.GetComponentInParent<TouchActor>();

            if (!actor)
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (no actor)");
                return;
            }
            
            if (Mute)
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (source mute)");
                return;
            }

            if (actor.Mute)
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (mute)");
                return;
            }

            if (_gatedActors.ContainsKey(actor))
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (gated actor)");
                return;
            }
            
            if (_touchData.TryGetValue(actor, out TouchData data))
            {
                if (data.Collider != other)
                {
                    this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (current actor with other collider)");
                    return;
                }
                if (_endOnReEnter)
                {
                    this.LogWarning($"{this} {nameof(OnTriggerEnter)} ended stream on re-enter)");
                    EndStream(data);
                    // Do not return _endOnReEnter implies restart, could change name to be more representative
                }
                else
                {
                    data.Colliding = true;
                    UpdateStream(data.Id, data);
                    this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (updated existing stream)");
                    return;
                }
            }

            if (!_actorMatch.Match(InteractionLayers, actor.InteractionLayers))
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (no actor layer mismatch)");
                return;
            }

            if (!IsCompatibleActor(actor))
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} bailed out (incompatible actor)");
                return;
            }
            
            if (!CheckGates(actor))
            {
                this.LogVerbose($"{this} {nameof(OnTriggerEnter)} gated actor");
                _gatedActors[actor] = other;
            }
            else
            {
                AttemptStream(actor, other);   
            }
        }

        private IEnumerable<TouchActor> GatedActors(Collider other) => 
            _gatedActors.Where(p => p.Value == other).Select(p => p.Key);

        protected virtual void OnTriggerStay(Collider other)
        {
            foreach (TouchActor actor in _reusableActors.Import(GatedActors(other)))
            {
                if (CheckGates(actor))
                {
                    this.LogWarning($"{this} {nameof(OnTriggerStay)} promoted gated actor");
                    _gatedActors.Remove(actor);
                    AttemptStream(actor, other);
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            this.LogVerbose($"{this} {nameof(OnTriggerExit)} {other}");

            foreach (TouchActor actor in _reusableActors.Import(GatedActors(other)))
            {
                _gatedActors.Remove(actor);
            }
            
            // should only be one 
            
            foreach (TouchData data in _reusableData.Import(_touchData.Values.Where(d => d.Collider == other)))
            {
                data.Colliding = false;
                if (_endOnExit)
                {
                    this.LogVerbose($"{this} {nameof(OnTriggerExit)} ending stream on exit {data.Id}");
                    EndStream(data);
                }     
            }
        }

        private bool CheckGates(TouchActor actor)
        {
            if (!Gates.All(gate => gate && gate.Check(this, actor)))
            {
                return false;
            }
            
            if (!actor.Gates.All(gate => gate && gate.Check(this, actor)))
            {
                return false;
            }

            return true;
        }
        
        private bool AttemptStream(TouchActor actor, Collider other)
        {
            if (!actor.RequestPermission(other))
            {
                this.LogVerbose($"{this} {nameof(AttemptStream)} bailed out (actor refused permission)");
                return false;
            }
            
            TouchData touchData = new TouchData()
            {
                Id = Guid.NewGuid(),
                Collider = other,
                Colliding = true,
                Actor = actor,
                Source = this
            };

            _touchData[touchData.Actor] = touchData;

            if (!ConfigureStream(touchData.Id, touchData))
            {
                this.LogWarning($"{this} {nameof(AttemptStream)} failed to started stream {touchData.Id}");
                EndStream(touchData);
                return false;
            }

            if (_autoEnd)
            {
                Observable.Timer(TimeSpan.FromSeconds(_autoEndDelay)).Subscribe(_ => EndStream(touchData));
            }

            this.LogWarning($"{this} {nameof(AttemptStream)} started stream {touchData.Id}");
            
            return true;
        }
        
        
        private void EndStream(TouchData touchData)
        {
            if (_touchData.Remove(touchData.Actor))
            {
                CleanupStream(touchData.Id, touchData);   
            }
        }

        protected abstract bool ConfigureStream(Guid id, ITouchData touchData);

        protected abstract void UpdateStream(Guid id, ITouchData touchData);

        protected abstract void CleanupStream(Guid id, ITouchData touchData);
    }
}