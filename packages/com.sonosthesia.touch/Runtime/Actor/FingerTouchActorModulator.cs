using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.Touch
{
    public class FingerTouchActorModulator : TouchActorModulator
    {
        [SerializeField] private Handedness _handedness;

        [SerializeField] private XRHandFingerID _finger;
        
        public override float Select(TouchActorModulationType modulationType)
        {
            if (modulationType == TouchActorModulationType.None)
            {
                return 0;
            }
            XRHandSubsystem subsystem = SubsystemHelper.Get<XRHandSubsystem>();
            if (subsystem == null)
            {
                return 0;
            }
            XRHand hand = _handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;
            hand.TryGetModulation(_finger, modulationType, out float result);
            return result;
        }
    }
}