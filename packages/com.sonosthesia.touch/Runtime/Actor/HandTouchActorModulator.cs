using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class HandTouchActorModulator : TouchActorModulator
    {
        private enum Strategy
        {
            Max,
            Average
        }
        
        [SerializeField] private Handedness _handedness;

        [SerializeField] private Strategy _strategy;

        private static readonly List<float> _values = new ();
        
        public override float Select(TouchActorModulationType modulationType)
        {
            if (modulationType == TouchActorModulationType.None)
            {
                return 0;
            }

            if (_handedness == Handedness.Invalid)
            {
                return 0;
            }
            
            XRHandSubsystem subsystem = SubsystemHelper.Get<XRHandSubsystem>();
            if (subsystem == null)
            {
                return 0;
            }
            
            XRHand hand = _handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;
            
            _values.Clear();
            
            if (hand.TryGetModulation(XRHandFingerID.Index, modulationType, out float index))
            {
                _values.Add(index);
            }
            if (hand.TryGetModulation(XRHandFingerID.Middle, modulationType, out float middle))
            {
                _values.Add(middle);
            }
            if (hand.TryGetModulation(XRHandFingerID.Ring, modulationType, out float ring))
            {
                _values.Add(ring);
            }
            if (hand.TryGetModulation(XRHandFingerID.Ring, modulationType, out float little))
            {
                _values.Add(little);
            }

            if (modulationType != TouchActorModulationType.Pinch)
            {
                if (hand.TryGetModulation(XRHandFingerID.Thumb, modulationType, out float thumb))
                {
                    _values.Add(thumb);
                }
            }

            return _strategy switch
            {
                Strategy.Max => _values.Max(),
                Strategy.Average => _values.Average(),
                _ => 0
            };
        }
    }
}