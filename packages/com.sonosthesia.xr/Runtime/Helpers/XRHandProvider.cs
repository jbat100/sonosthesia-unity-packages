using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.XR
{
    public class XRHandProvider : MonoBehaviour, ILogSwitch
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
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
                this.LogError($"{this} no {nameof(TrackingEvents)}");
                return false;
            }

            if (_trackingEvents.handedness == Handedness.Invalid)
            {
                this.LogError($"{this} {_trackingEvents.handedness}");
                return false;
            }

            XRHandSubsystem subsystem = _trackingEvents.subsystem;

            if (subsystem == null)
            {
                this.LogError($"{this} no {_trackingEvents.subsystem}");
                return false;
            }

            hand = _trackingEvents.handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;

            if (!hand.isTracked)
            {
                this.LogError($"{this} not tracked");
                return false;
            }

            return true;
        }
    }
}