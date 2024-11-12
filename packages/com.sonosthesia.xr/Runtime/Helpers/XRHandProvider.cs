using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.XR
{
    public class XRHandProvider : MonoBehaviour
    {
        [SerializeField] private XRHandTrackingEvents _trackingEvents;
        public XRHandTrackingEvents TrackingEvents => _trackingEvents;

        protected void Awake()
        {
            if (!_trackingEvents)
            {
                _trackingEvents = GetComponent<XRHandTrackingEvents>();
            }
        }

        public bool TryGetTrackedHand(out XRHand hand)
        {
            hand = default;
            
            if (!_trackingEvents)
            {
                return false;
            }

            if (_trackingEvents.handedness == Handedness.Invalid)
            {
                return false;
            }

            XRHandSubsystem subsystem = _trackingEvents.subsystem;

            if (subsystem == null)
            {
                return false;
            }

            hand = _trackingEvents.handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;

            if (!hand.isTracked)
            {
                return false;
            }

            return true;
        }
    }
}