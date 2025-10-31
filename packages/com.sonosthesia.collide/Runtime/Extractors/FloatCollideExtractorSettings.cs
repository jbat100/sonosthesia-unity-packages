using System;
using Sonosthesia.Extractor;
using Sonosthesia.Interaction;

namespace Sonosthesia.Collide
{
    [Serializable]
    public class FloatCollideStaticExtractorSettings : 
        FloatInteractionStaticExtractorSettings<CollideEvent, FloatPostProcessingSettings>
    {
        
    }
    
    [Serializable]
    public class FloatCollideDynamicExtractorSettings : 
        FloatInteractionDynamicExtractorSettings<CollideEvent>
    {

    }
}