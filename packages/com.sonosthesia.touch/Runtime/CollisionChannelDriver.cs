using System.Collections.Generic;
using Sonosthesia.Flow;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [RequireComponent(typeof(RigidTransformVelocity))]
    public abstract class CollisionChannelDriver<TPayload> : MonoBehaviour where TPayload : struct
    {
         [SerializeField] private Channel<TPayload> _channel;

        private RigidTransformVelocity _velocity;
        private readonly Dictionary<Transform, Subject<TPayload>> _subjects = new();

        protected RigidTransformVelocity Velocity => _velocity;

        protected virtual void Awake()
        {
            _velocity = GetComponent<RigidTransformVelocity>();
        }
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionEnter)} {collision.ToDetailedString()}");
            Subject<TPayload> subject = new Subject<TPayload>();
            _subjects[collision.transform] = subject;
            _channel.Pipe(subject);
            if (MakePayload(collision, out TPayload payload))
            {
                subject.OnNext(payload);
            }
        }
        
        protected virtual void OnCollisionStay(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionStay)} {collision.ToDetailedString()}");
            if (_subjects.TryGetValue(collision.transform, out Subject<TPayload> subject))
            {
                if (MakePayload(collision, out TPayload payload))
                {
                    subject.OnNext(payload);
                }
            }
        }
        
        protected virtual void OnCollisionExit(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionExit)} {collision.ToDetailedString()}");
            if (_subjects.TryGetValue(collision.transform, out Subject<TPayload> subject))
            {
                subject.OnCompleted();
                subject.Dispose();
            }
        }

        protected abstract bool MakePayload(Collision collision, out TPayload payload);
    }
}