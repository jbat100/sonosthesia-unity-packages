using System.Collections.Generic;
using Sonosthesia.Flow;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [RequireComponent(typeof(TransformDynamicsMonitor))]
    public abstract class CollisionChannelDriver<TPayload> : MonoBehaviour where TPayload : struct
    {
         [SerializeField] private Channel<TPayload> _channel;

        private TransformDynamicsMonitor _dynamicsMonitor;
        private readonly Dictionary<Transform, Subject<TPayload>> _subjects = new();
        private readonly Dictionary<Transform, TransformDynamicsMonitor> _monitors = new();

        protected virtual void Awake()
        {
            _dynamicsMonitor = GetComponent<TransformDynamicsMonitor>();
        }
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"{this} {nameof(OnCollisionEnter)} {collision.ToDetailedString()}");
            Subject<TPayload> subject = new Subject<TPayload>();
            _subjects[collision.transform] = subject;
            _monitors[collision.transform] = collision.transform.GetComponent<TransformDynamicsMonitor>();
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
            _subjects.Remove(collision.transform);
            _monitors.Remove(collision.transform);
        }

        private bool MakePayload(Collision collision, out TPayload payload)
        {
            if (_monitors.TryGetValue(collision.transform, out TransformDynamicsMonitor monitor) && monitor)
            {
                return MakePayload(collision, _dynamicsMonitor.Dynamics - monitor.Dynamics, out payload);
            }
            return MakePayload(collision, _dynamicsMonitor.Dynamics, out payload);
        }


        protected abstract bool MakePayload(Collision collision, TransformDynamics transformDynamics, out TPayload payload);
    }
}