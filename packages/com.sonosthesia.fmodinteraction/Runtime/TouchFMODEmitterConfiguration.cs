using Sonosthesia.Interaction;
using UnityEngine;
using Sonosthesia.Touch;

namespace Sonosthesia.FMODInteraction
{
    [CreateAssetMenu(fileName = "TouchFMODEmitterAffordance", menuName = "Sonosthesia/Touch/TouchFMODEmitterAffordance")]
    public class TouchFMODEmitterConfiguration : FMODEmitterConfiguration<TouchEvent, 
        InteractiveEnvelopeSettings<TouchEvent, FloatTouchDynamicExtractorSettings, FloatTouchStaticExtractorSettings>>
    {

    }
}