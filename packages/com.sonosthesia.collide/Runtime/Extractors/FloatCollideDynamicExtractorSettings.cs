using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Collide
{
    [Serializable]
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