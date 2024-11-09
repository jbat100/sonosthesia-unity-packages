using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.Touch
{
    public class HandTouchActor : TouchActor
    {
        [SerializeField] private XRHandTrackingEvents _trackingEvents;
        public XRHandTrackingEvents TrackingEvents => _trackingEvents;
    }
}