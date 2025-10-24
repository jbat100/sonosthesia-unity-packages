using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Collide
{
    [Serializable]
    public class FloatCollideStaticExtractorSettings : StaticExtractorSettings<CollideEvent, float, FloatPostProcessingSettings>
    {
        protected override bool ExtractRaw(CollideEvent e, out float value)
        {
            value = 0f;
            return false;
        }
    }
}