using Sonosthesia.Pointer;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "PointerMPENoteConfiguration", menuName = "Sonosthesia/Instrument/PointerMPENoteConfiguration")]
    public class PointerMPENoteConfiguration : MPENoteConfiguration<PointerEvent, MIDIPitchPointerExtractorSettings, 
        FloatPointerStaticExtractorSettings, FloatPointerDynamicExtractorSettings>
    {
        
    }
}