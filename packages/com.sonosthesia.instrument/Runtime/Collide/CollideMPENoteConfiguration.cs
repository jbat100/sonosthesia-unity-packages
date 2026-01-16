using Sonosthesia.Collide;
using UnityEngine;

namespace Sonosthesia.Instrument.Collide
{
    [CreateAssetMenu(fileName = "CollideMPENoteConfiguration", menuName = "Sonosthesia/Instrument/CollideMPENoteConfiguration")]
    public class CollideMPENoteConfiguration : MPENoteConfiguration<CollideEvent,
        MIDIPitchExtractorSettings<CollideEvent>, FloatCollideStaticExtractorSettings, FloatCollideDynamicExtractorSettings>
    {
        
    }
}