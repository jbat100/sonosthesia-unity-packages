using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "TouchMPENoteConfiguration", menuName = "Sonosthesia/Instrument/TouchMPENoteConfiguration")]
    public class TouchMPENoteConfiguration : MPENoteConfiguration<TouchEvent, 
        MIDIPitchTouchExtractorSettings, FloatTouchStaticExtractorSettings, FloatTouchDynamicExtractorSettings>
    {

    }
}