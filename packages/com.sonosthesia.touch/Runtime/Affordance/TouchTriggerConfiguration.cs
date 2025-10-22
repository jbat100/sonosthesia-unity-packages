using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchTriggerConfiguration", menuName = "Sonosthesia/Touch/TouchTriggerConfiguration")]
    public class TouchTriggerConfiguration : TriggerConfiguration<TouchEvent, 
        FloatTouchDynamicExtractorSettings, 
        FloatTouchStaticExtractorSettings>
    {
        
    }
}