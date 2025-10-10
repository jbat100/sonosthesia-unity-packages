using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class TouchEnvelopeSettings : InteractiveEnvelopeSettings<TouchEvent, 
        FloatDynamicExtractorSettings<TouchEvent>, 
        FloatStaticExtractorSettings<TouchEvent>>
    {
       
    }
}