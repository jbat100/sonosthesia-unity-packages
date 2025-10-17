using Sonosthesia.Pointer;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "PointerMIDINoteConfiguration", menuName = "Sonosthesia/Instrument/PointerMIDINoteConfiguration")]
    public class PointerMIDINoteConfiguration : MIDINoteConfiguration<PointerEvent, 
        MIDIChannelPointerExtractorSettings, MIDIPitchPointerExtractorSettings, 
        FloatPointerStaticExtractorSettings, FloatPointerDynamicExtractorSettings>
    {
        
    }
}