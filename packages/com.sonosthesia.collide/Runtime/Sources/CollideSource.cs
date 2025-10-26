using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Collide
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(CollideSource), true)]
    public class CollideSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CollideSource source = (CollideSource)target;
            if(GUILayout.Button("Debug State"))
            {
                source.DebugState();
            }
        }
    }
#endif
    
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

            public bool IsObsolete => EndTime.HasValue && EndTime.Value < Time.time;
            
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

        internal void DebugState()
        {
            Debug.Log($"{this} has {_collisionData.Count} collision data and {_residueData.Count} residue data");
        }
        
        private readonly Dictionary<Collision, CollideData> _collisionData = new();
        private readonly HashSet<CollideData> _residueData = new();
        
        private static readonly HashSet<CollideData> _obsoleteData = new();
        private static readonly HashSet<Collision> _obsoleteCollisions = new();
        
        protected virtual void FixedUpdate()
        {
            _obsoleteCollisions.Clear();
            foreach (KeyValuePair<Collision, CollideData> pair in _collisionData)
            {
                if (pair.Value.IsObsolete)
                {
                    pair.Value.End();
                    _obsoleteCollisions.Add(pair.Key);
                }
            }
            foreach (Collision collision in _obsoleteCollisions)
            {
                _collisionData.Remove(collision);   
            }
            
            _obsoleteData.Clear();
            foreach (CollideData data in _residueData)
            {
                Debug.Log($"{this} checking residue data end time {data.EndTime} time is {Time.time}");
                if (data.IsObsolete)
                {
                    data.End();
                    _obsoleteData.Add(data);
                }
                else
                {
                    data.Subject.OnNext(new CollideEvent(null, data.StartTime, this, data.Actor));
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

            if (_collisionData.ContainsKey(collision))
            {
                this.LogWarning($"{this} {nameof(OnCollisionEnter)} already tracking collision {collision}");
                return;
            }
            
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