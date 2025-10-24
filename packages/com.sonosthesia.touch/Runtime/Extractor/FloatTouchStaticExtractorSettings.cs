using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchStaticExtractorSettings : StaticExtractorSettings<TouchEvent, float, FloatPostProcessingSettings>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Velocity,
            Distance
        }

        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;
        
        [SerializeField] private VelocityExtractionType _velocityType = VelocityExtractionType.Actor;
        
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
                    return e.ExtractVelocity(_velocityType, out value);
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