using UnityEngine;
using Sonosthesia.Touch;

namespace Sonosthesia.DeformInteraction
{
    [CreateAssetMenu(fileName = "TouchPathNoiseConfiguration", menuName = "Sonosthesia/Touch/TouchPathNoiseConfiguration")]
    public class TouchPathNoiseConfiguration : PathNoiseConfiguration<TouchEvent, TouchEnvelopeSettings>
    {
        
    }
}