using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Collide
{
    [CreateAssetMenu(fileName = "CollideTriggerConfiguration", menuName = "Sonosthesia/Collide/CollideTriggerConfiguration")]
    public class CollideTriggerConfiguration : TriggerConfiguration<CollideEvent, 
        FloatCollideDynamicExtractorSettings, 
        FloatCollideStaticExtractorSettings>
    {
        
    }
}