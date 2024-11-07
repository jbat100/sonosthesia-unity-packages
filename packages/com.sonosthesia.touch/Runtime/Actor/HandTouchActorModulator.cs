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

        [SerializeField] private bool _thumb = true;
        [SerializeField] private bool _index = true;
        [SerializeField] private bool _middle = true;
        [SerializeField] private bool _ring = true;
        [SerializeField] private bool _little = true;

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
            
            if (_index && hand.TryGetModulation(XRHandFingerID.Index, modulationType, out float index))
            {
                _values.Add(index);
            }
            if (_middle && hand.TryGetModulation(XRHandFingerID.Middle, modulationType, out float middle))
            {
                _values.Add(middle);
            }
            if (_ring && hand.TryGetModulation(XRHandFingerID.Ring, modulationType, out float ring))
            {
                _values.Add(ring);
            }
            if (_little && hand.TryGetModulation(XRHandFingerID.Ring, modulationType, out float little))
            {
                _values.Add(little);
            }

            if (modulationType != TouchActorModulationType.Pinch)
            {
                if (_thumb && hand.TryGetModulation(XRHandFingerID.Thumb, modulationType, out float thumb))
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