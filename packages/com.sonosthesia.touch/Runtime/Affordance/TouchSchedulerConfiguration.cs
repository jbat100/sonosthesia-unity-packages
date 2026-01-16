using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    
    [CreateAssetMenu(fileName = "TouchSchedulerConfiguration", menuName = "Sonosthesia/Touch/TouchSchedulerConfiguration")]
    public class TouchSchedulerConfiguration : SchedulerConfiguration<TouchEvent, 
        InteractiveEnvelopeSettings<TouchEvent, FloatTouchDynamicExtractorSettings, FloatTouchStaticExtractorSettings>>
    {

    }
}