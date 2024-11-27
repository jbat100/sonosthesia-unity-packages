using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.XR.Hands;
using Sonosthesia.XR;

namespace Sonosthesia.Touch
{
    public class XRHandTouchActorModulator : TouchActorModulator
    {
        private enum Strategy
        {
            Max,
            Average
        }

        [SerializeField] private XRHandProvider _hand;

        [SerializeField] private Strategy _strategy;

        [SerializeField] private bool _thumb = true;
        [SerializeField] private bool _index = true;
        [SerializeField] private bool _middle = true;
        [SerializeField] private bool _ring = true;
        [SerializeField] private bool _little = true;

        private static readonly List<float> _values = new ();
        
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
                this.LogVerbose($"{this} {nameof(Select)} bailing out on none");
                return 0;
            }

            if (!_hand.TryGetTrackedHand(out XRHand hand))
            {
                this.LogVerbose($"{this} {nameof(Select)} bailing out on failed {nameof(_hand.TryGetTrackedHand)}");
                return 0;
            }
            
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
            
            this.LogVerbose($"{this} {nameof(Select)} computing from {string.Join(", ", _values)}");

            return _strategy switch
            {
                Strategy.Max => _values.Max(),
                Strategy.Average => _values.Average(),
                _ => 0
            };
        }
    }
}