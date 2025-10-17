using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchStaticExtractorSettings : FloatStaticExtractorSettings<TouchEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Velocity,
            Distance
        }

        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;
        
        [SerializeField] private TouchVelocityType _velocityType = TouchVelocityType.Actor;
        
        [SerializeField] private Axes _distanceAxes = Axes.X | Axes.Y | Axes.Z;
        
        protected override bool ExtractRaw(TouchEvent e, out float value)
        {
            switch (_extractorType)
            {
                case ExtractorType.Custom:
                    return Custom(e, out value);
                case ExtractorType.Constant:
                    value = ConstantValue;
                    return true; 
                case ExtractorType.Velocity:
                    return TouchExtractionUtils.ExtractVelocity(e, _velocityType, out value);
                case ExtractorType.Distance:
                    value = e.ActorPositionInSourceSpace().FilterAxes(_distanceAxes).magnitude;
                    return true;
                default:
                    value = 0;
                    return false;
            }
        }
    }
}