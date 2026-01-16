using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "PointerTriggerConfiguration", menuName = "Sonosthesia/Pointer/PointerTriggerConfiguration")]
    public class PointerTriggerConfiguration : TriggerConfiguration<PointerEvent, 
        FloatPointerDynamicExtractorSettings, 
        FloatPointerStaticExtractorSettings>
    {
        
    }
}