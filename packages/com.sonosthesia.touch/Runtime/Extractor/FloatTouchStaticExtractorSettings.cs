using System;
using Sonosthesia.Extractor;
using Sonosthesia.Interaction;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class FloatTouchStaticExtractorSettings 
        : FloatInteractionStaticExtractorSettings<TouchEvent, FloatPostProcessingSettings>
    {
        
    }
}