using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [Serializable]
    public class FloatPointerStaticExtractorSettings : StaticExtractorSettings<PointerEvent, float, FloatPostProcessingSettings>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Pressure,
            Raycast,
            Screen
        }
        
        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;
        
        [SerializeField] private PointerRaycastSpace _space = PointerRaycastSpace.Target;
        
        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;

        [SerializeField] private VectorFloatSelector _selector;

        protected bool ExtractPressure(PointerEvent e, out float value)
        {
            value = e.Data.pressure;
            return true;
        }
        
        protected override bool ExtractRaw(PointerEvent e, out float value) => _extractorType switch
        {
            ExtractorType.Custom => ExtractCustom(e, out value),
            ExtractorType.Constant => ExtractConstant(e, out value),
            ExtractorType.Pressure => ExtractPressure(e, out value),
            ExtractorType.Raycast => e.ExtractScreen(_axes, _selector, out value),
            ExtractorType.Screen => e.ExtractRaycast(_space, _axes, _selector, out value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}