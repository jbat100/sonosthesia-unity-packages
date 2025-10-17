using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Pointer
{
    [Serializable]
    public class PointerEnvelopeSettings : InteractiveEnvelopeSettings<PointerEvent, 
        FloatPointerDynamicExtractorSettings, 
        FloatPointerStaticExtractorSettings>
    {

    }
}