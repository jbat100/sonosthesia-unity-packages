using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    public class FloatPointerStaticExtractorSettings : FloatStaticExtractorSettings<PointerEvent>
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

        protected override bool ExtractRaw(PointerEvent e, out float value)
        {
            switch (_extractorType)
            {
                case ExtractorType.Custom:
                    return Custom(e, out value);
                case ExtractorType.Constant:
                    value = ConstantValue;
                    return true;
                case ExtractorType.Pressure:
                    value = e.Data.pressure;
                    return true;
                case ExtractorType.Screen:
                    return e.ExtractScreen(_axes, out value);
                case ExtractorType.Raycast:
                    return e.ExtractRaycast(_space, _axes, out value);
            }
            
            value = 0f;
            return false;
        }
    }
}