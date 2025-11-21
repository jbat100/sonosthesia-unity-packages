using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    [CreateAssetMenu(fileName = "TouchMeshNoiseConfiguration", menuName = "Sonosthesia/Touch/TouchMeshNoiseConfiguration")]
    public class TouchMeshNoiseConfiguration : MeshNoiseConfiguration<TouchEvent, 
        InteractiveEnvelopeSettings<TouchEvent, FloatTouchDynamicExtractorSettings, FloatTouchStaticExtractorSettings>>
    {

    }
}