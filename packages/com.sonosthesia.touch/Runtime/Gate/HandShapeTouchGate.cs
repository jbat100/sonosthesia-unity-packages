using System;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace Sonosthesia.Touch
{
    public class HandShapeTouchGate : TouchGate
    {
        [SerializeField] private XRHandShape _shape;

        [SerializeField] private XRHandTrackingEvents _trackingEvents;

        private IDisposable _subscription;
        private bool _match;
        
        protected virtual void OnEnable()
        {
            _match = false;
            _subscription?.Dispose();

            if (!_trackingEvents)
            {
                return;
            }
            
            _subscription = _trackingEvents.jointsUpdated.AsObservable().Subscribe(eventArgs =>
            {
                _match = _shape && _shape.CheckConditions(eventArgs);
            });
        }

        protected virtual void OnDisable()
        {
            _match = false;
            _subscription?.Dispose();
        }

        protected override bool PerformCheck(TouchSource source, TouchActor actor)
        {
            return _match;
        }
    }
}