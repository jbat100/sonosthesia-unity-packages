using System;
using Sonosthesia.Interaction;
using Sonosthesia.Processing;

namespace Sonosthesia.Collide
{
    [Serializable]
    public class FloatCollideStaticExtractorSettings : 
        FloatInteractionStaticExtractorSettings<CollideEvent, FloatProcessorSettings>
    {
        
    }
    
    [Serializable]
    public class FloatCollideDynamicExtractorSettings : 
        FloatInteractionDynamicExtractorSettings<CollideEvent>
    {

    }
}