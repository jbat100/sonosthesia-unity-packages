using UnityEngine;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;

namespace Sonosthesia.DeformInteraction
{
    [CreateAssetMenu(fileName = "TouchPathNoiseConfiguration", menuName = "Sonosthesia/Touch/TouchPathNoiseConfiguration")]
    public class TouchPathNoiseConfiguration : PathNoiseConfiguration<TouchEvent, 
        InteractiveEnvelopeSettings<TouchEvent, FloatTouchDynamicExtractorSettings, FloatTouchStaticExtractorSettings>>
    {
        
    }
}