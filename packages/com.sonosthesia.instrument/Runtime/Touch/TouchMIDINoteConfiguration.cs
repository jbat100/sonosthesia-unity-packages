using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "TouchMIDINoteConfiguration", menuName = "Sonosthesia/Instrument/TouchMIDINoteConfiguration")]
    public class TouchMIDINoteConfiguration : MIDINoteConfiguration<TouchEvent, 
        MIDIChannelTouchExtractorSettings, MIDIPitchTouchExtractorSettings, 
        FloatTouchStaticExtractorSettings, FloatTouchDynamicExtractorSettings> 
    {
        
    }
}