using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Collide
{
    [Serializable]
    public class CollideEnvelopeSettings : InteractiveEnvelopeSettings<CollideEvent, 
        FloatCollideDynamicExtractorSettings, 
        FloatCollideStaticExtractorSettings>
    {
        
    }
}