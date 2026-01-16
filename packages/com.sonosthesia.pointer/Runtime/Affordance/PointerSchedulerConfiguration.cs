using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "PointerSchedulerConfiguration", menuName = "Sonosthesia/Pointer/PointerSchedulerConfiguration")]
    public class PointerSchedulerConfiguration : SchedulerConfiguration<PointerEvent, 
        InteractiveEnvelopeSettings<PointerEvent, FloatPointerDynamicExtractorSettings, FloatPointerStaticExtractorSettings>>
    {
        
    }
}