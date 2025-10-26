using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchTorqueConfiguration", menuName = "Sonosthesia/Touch/TouchTorqueConfiguration")]
    public class TouchTorqueConfiguration : TorqueConfiguration<TouchEvent, TouchEnvelopeSettings, VectorTouchDynamicExtractorSettings>
    {
        
    }
}