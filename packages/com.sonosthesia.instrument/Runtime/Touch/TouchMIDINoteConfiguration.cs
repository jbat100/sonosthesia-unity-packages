using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "TouchMIDINoteConfiguration", 
        menuName = "Sonosthesia/Instrument/TouchMIDINoteConfiguration")]
    public class TouchMIDINoteConfiguration : MIDINoteConfiguration<TouchEvent, 
        MIDIChannelExtractorSettings<TouchEvent>, MIDIPitchExtractorSettings<TouchEvent>, 
        FloatTouchStaticExtractorSettings, FloatTouchDynamicExtractorSettings> 
    {
        
    }
}