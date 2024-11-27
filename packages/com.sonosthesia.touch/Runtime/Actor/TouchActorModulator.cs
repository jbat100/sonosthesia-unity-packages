using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace Sonosthesia.Touch
{
    // note : this is deliberately generic, with different providing strategies implemented by TouchActor subclasses
    // we sacrifice specificity for simplicity 

    public enum TouchActorModulationType
    {
        None,
        Curl,
        Tension,
        Pinch
    }

    public abstract class TouchActorModulator : MonoBehaviour, ILogSwitch
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        public abstract float Select(TouchActorModulationType modulationType);
    }

    public static class TouchActorModulationExtension
    {
        public static bool TryGetModulation(this XRHand hand, XRHandFingerID finger, TouchActorModulationType modulationType, out float result)
        {
            XRFingerShapeTypes shapeTypes = modulationType switch
            {
                TouchActorModulationType.Curl => XRFingerShapeTypes.FullCurl,
                TouchActorModulationType.Tension => XRFingerShapeTypes.TipCurl | XRFingerShapeTypes.BaseCurl,
                TouchActorModulationType.Pinch => XRFingerShapeTypes.Pinch,
                _ => XRFingerShapeTypes.None
            };
            
            XRFingerShape shape = hand.CalculateFingerShape(finger, shapeTypes);

            switch (modulationType)
            {
                case TouchActorModulationType.Curl:
                    if (shape.TryGetFullCurl(out float fullCurl))
                    {
                        result = fullCurl;
                        return true;
                    }
                    break;
                case TouchActorModulationType.Tension:
                    if (shape.TryGetTipCurl(out float tipCurl) && shape.TryGetBaseCurl(out float baseCurl))
                    {
                        result = math.abs(tipCurl - baseCurl);
                        return true;
                    }
                    break;
                case TouchActorModulationType.Pinch:
                    if (shape.TryGetPinch(out float pinch))
                    {
                        result = pinch;
                        return true;
                    }
                    break;
            }

            result = 0;
            return false;
        }
    }
}