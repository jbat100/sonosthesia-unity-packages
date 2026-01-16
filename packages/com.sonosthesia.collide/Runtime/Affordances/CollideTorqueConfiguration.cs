using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Collide
{
    [CreateAssetMenu(fileName = "CollideTorqueConfiguration", menuName = "Sonosthesia/Collide/CollideTorqueConfiguration")]
    public class CollideTorqueConfiguration : TorqueConfiguration<CollideEvent, CollideEnvelopeSettings, VectorCollideDynamicExtractorSettings>
    {
        
    }
}