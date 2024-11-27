using System;
using Sonosthesia.XR;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Hands.Gestures;

namespace Sonosthesia.Touch
{
    public class XRHandShapeTouchActorGate : TouchActorGate
    {
        [SerializeField] private XRHandShape _shape;

        [SerializeField] private XRHandProvider _hand;

        private IDisposable _subscription;
        private bool _match;

        protected virtual void Awake()
        {
            if (!_hand)
            {
                _hand = GetComponentInParent<XRHandProvider>();
            }
        }
        
        protected virtual void OnEnable()
        {
            _match = false;
            _subscription?.Dispose();

            if (!_hand || !_hand.TrackingEvents)
            {
                return;
            }
            
            _subscription = _hand.TrackingEvents.jointsUpdated.AsObservable().Subscribe(eventArgs =>
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