using Sonosthesia.Interaction;

namespace Sonosthesia.Collide
{
    public class FloatCollideStaticExtractorSettings : FloatStaticExtractorSettings<CollideEvent>
    {
        protected override bool ExtractRaw(CollideEvent e, out float value)
        {
            value = 0f;
            return false;
        }
    }
}