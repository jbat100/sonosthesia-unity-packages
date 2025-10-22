using Sonosthesia.Interaction;

namespace Sonosthesia.Collide
{
    public class FloatCollideDynamicExtractorSettings : FloatDynamicExtractorSettings<CollideEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Velocity
        }
        
        
        
        protected override IDynamicExtractorSession<CollideEvent, float> MakeRawSession()
        {
            return null;
        }
    }
}