using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [RequireComponent(typeof(RigidTransformVelocity))]
    public class ColliderTouchDriver : MonoBehaviour
    {
        [SerializeField] private TouchChannel _channel;

        private RigidTransformVelocity _velocity;
        private readonly Dictionary<Transform, Subject<TouchPayload>> _subjects = new();

        protected virtual void Awake()
        {
            _velocity = GetComponent<RigidTransformVelocity>();
        }
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionEnter)} {collision.ToDetailedString()}");
            Subject<TouchPayload> subject = new Subject<TouchPayload>();
            _subjects[collision.transform] = subject;
            _channel.Pipe(subject);
            if (MakePayload(collision, out TouchPayload payload))
            {
                subject.OnNext(payload);
            }
        }
        
        protected virtual void OnCollisionStay(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionStay)} {collision.ToDetailedString()}");
            if (_subjects.TryGetValue(collision.transform, out Subject<TouchPayload> subject))
            {
                if (MakePayload(collision, out TouchPayload payload))
                {
                    subject.OnNext(payload);
                }
            }
        }
        
        protected virtual void OnCollisionExit(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionExit)} {collision.ToDetailedString()}");
            if (_subjects.TryGetValue(collision.transform, out Subject<TouchPayload> subject))
            {
                subject.OnCompleted();
                subject.Dispose();
            }
        }

        private bool MakePayload(Collision collision, out TouchPayload payload)
        {
            if (_velocity && collision.contacts.Length > 0)
            {
                Transform t = transform;
                payload = new TouchPayload(collision.contacts[0], t.ToRigidTransform(), _velocity.Velocity);
                return true;
            }
            payload = default;
            return false;
        }
    }
}