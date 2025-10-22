using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Dynamic;
using Sonosthesia.Interaction;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Collide
{
    [RequireComponent(typeof(TransformDynamicsMonitor))]
    public abstract class CollideSource : InteractionEndpoint
    {
        [SerializeField] private Channel<CollideEvent> _channel;
        
        private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;

        private class CollideData
        {
            public BehaviorSubject<CollideEvent> Subject;
            public CollideActor Actor;
            public float StartTime;
        }
        
        private readonly Dictionary<Collision, CollideData> _data = new();

        protected virtual void Awake()
        {
            _dynamicsMonitor = GetComponent<TransformDynamicsMonitor>();
        }
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionEnter)} {collision.ToDetailedString()}");
            
            CollideActor actor = collision.gameObject.GetComponent<CollideActor>();

            if (!actor)
            {
                return;
            }
            
            CollideEvent collideEvent = new CollideEvent(collision, Time.time, this, actor);
            BehaviorSubject<CollideEvent> subject = new (collideEvent);
            
            _data[collision] = new CollideData()
            {
                Subject = subject
            };            
            
            _channel.Push(Guid.NewGuid(), subject);
        }
        
        protected virtual void OnCollisionStay(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionStay)} {collision.ToDetailedString()}");
            if (_data.TryGetValue(collision, out CollideData data))
            {
                data.Subject.OnNext(new CollideEvent(collision, data.StartTime, this, data.Actor));
            }
        }
        
        protected virtual void OnCollisionExit(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionExit)} {collision.ToDetailedString()}");
            if (_data.TryGetValue(collision, out CollideData data))
            {
                data.Subject.OnCompleted();
                data.Subject.Dispose();
            }
            _data.Remove(collision);
            _data.Remove(collision);
        }
    }
}