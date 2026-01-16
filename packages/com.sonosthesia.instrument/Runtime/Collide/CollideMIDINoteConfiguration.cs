using Sonosthesia.Collide;
using UnityEngine;

namespace Sonosthesia.Instrument.Collide
{
    [CreateAssetMenu(fileName = "CollideMIDINoteConfiguration", menuName = "Sonosthesia/Instrument/CollideMIDINoteConfiguration")]
    public class CollideMIDINoteConfiguration : MIDINoteConfiguration<CollideEvent,
        MIDIChannelExtractorSettings<CollideEvent>, MIDIPitchExtractorSettings<CollideEvent>, 
        FloatCollideStaticExtractorSettings, FloatCollideDynamicExtractorSettings>
    {
        
    }
}