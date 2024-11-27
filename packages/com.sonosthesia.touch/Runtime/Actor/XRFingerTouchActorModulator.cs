using UnityEngine;
using UnityEngine.XR.Hands;
using Sonosthesia.XR;

namespace Sonosthesia.Touch
{
    public class XRFingerTouchActorModulator : TouchActorModulator
    {
        [SerializeField] private XRHandProvider _hand;

        [SerializeField] private XRHandFingerID _finger;

        protected virtual void Awake()
        {
            if (!_hand)
            {
                _hand = GetComponentInParent<XRHandProvider>();
            }
        }
        
        public override float Select(TouchActorModulationType modulationType)
        {
            if (modulationType == TouchActorModulationType.None)
            {
                return 0;
            }

            if (_hand && _hand.TryGetTrackedHand(out XRHand hand))
            {
                if (hand.TryGetModulation(_finger, modulationType, out float result))
                {
                    return result;
                }
            }
            
            return 0;
        }
    }
}