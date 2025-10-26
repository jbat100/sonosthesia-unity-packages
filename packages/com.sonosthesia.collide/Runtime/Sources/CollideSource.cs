using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Collide
{
    public class CollideSource : InteractionEndpoint
    {
        [SerializeField] private Channel<CollideEvent> _channel;

        public enum EndStrategy
        {
            Auto,
            Exit
        }
        
        [SerializeField] private EndStrategy _endStrategy;
        [SerializeField] private float _autoEndDelay = 1;

        private class CollideData
        {
            public BehaviorSubject<CollideEvent> Subject;
            public CollideActor Actor;
            public float StartTime;
            public float? EndTime;

            public void End()
            {
                if (Subject == null)
                {
                    return;
                }
                Subject.OnCompleted();
                Subject.Dispose();
                Subject = null;
            }
        }
        
        private readonly Dictionary<Collision, CollideData> _collisionData = new();
        private readonly HashSet<CollideData> _residueData = new();
        
        private static readonly HashSet<CollideData> _obsoleteData = new();

        protected virtual void FixedUpdate()
        {
            _obsoleteData.Clear();
            foreach (CollideData data in _residueData)
            {
                if (data.EndTime.HasValue && data.EndTime.Value < Time.time)
                {
                    data.End();
                    _obsoleteData.Add(data);
                }
            }
            _residueData.ExceptWith(_obsoleteData);
        }

        protected virtual void OnDisable()
        {
            foreach (CollideData data in _collisionData.Values)
            {
                data.End();
            }
            _collisionData.Clear();
            foreach (CollideData data in _residueData)
            {
                data.End();
            }
            _residueData.Clear();
        }
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            this.LogVerbose($"{this} {nameof(OnCollisionEnter)} {collision.ToDetailedString()}");
            
            CollideActor actor = collision.gameObject.GetComponent<CollideActor>();

            if (!actor)
            {
                return;
            }
            
            CollideEvent collideEvent = new CollideEvent(collision, Time.time, this, actor);
            BehaviorSubject<CollideEvent> subject = new (collideEvent);
            
            _collisionData[collision] = new CollideData()
            {
                Subject = subject,
                Actor = actor,
                StartTime = Time.time,
                EndTime = _endStrategy == EndStrategy.Auto ? Time.time + _autoEndDelay : null
            };            
            
            _channel.Push(Guid.NewGuid(), subject);
        }
        
        protected virtual void OnCollisionStay(Collision collision)
        {
            this.LogVerbose($"{this} {nameof(OnCollisionStay)} {collision.ToDetailedString()}");

            if (!_collisionData.TryGetValue(collision, out CollideData data))
            {
                return;
            }
            
            data.Subject.OnNext(new CollideEvent(collision, data.StartTime, this, data.Actor));
            
            if (data.EndTime.HasValue && Time.time >= data.EndTime.Value)
            {
                data.End();
                _collisionData.Remove(collision);
            }
        }
        
        protected virtual void OnCollisionExit(Collision collision)
        {
            this.LogVerbose($"{this} {nameof(OnCollisionExit)} {collision.ToDetailedString()}");

            if (!_collisionData.TryGetValue(collision, out CollideData data))
            {
                return;
            }
            
            if (_endStrategy == EndStrategy.Exit)
            {
                data.End();
            }
            else
            {
                _residueData.Add(data);
            }
            
            _collisionData.Remove(collision);
        }
    }
}