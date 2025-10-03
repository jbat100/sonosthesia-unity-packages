using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [Serializable]
    public class FloatPointerExtractorSettings : FloatExtractorSettings<PointerEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Static,
            Pressure,
            Scroll,
            Drag
        }
        
        private enum AxisType
        {
            Raycast,
            Position
        }
        
        [SerializeField] private ExtractorType _extractorType = ExtractorType.Static;
        
        [SerializeField] private bool _relative;
        
        [SerializeField] private bool _track;
        
        [SerializeField] private AxisType _axisType;

        public override IExtractorSession<PointerEvent, float> MakeSession()
        {
            IExtractorSession<PointerEvent, float> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Static => StaticSession(),
                ExtractorType.Pressure => new PressureSession(),
                // ExtractorType.Scroll => expr,
                // ExtractorType.Drag => expr,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return null;
        }
        
        private class PressureSession : IExtractorSession<PointerEvent, float>
        {
            private static bool Common(PointerEvent pointerEvent, out float value)
            {
                value = pointerEvent.Data.pressure;
                return true;
            }
            
            public bool Setup(PointerEvent pointerEvent, out float value) => Common(pointerEvent, out value);

            public bool Update(PointerEvent touchEvent, out float value) => Common(touchEvent, out value);
        }
    }
}