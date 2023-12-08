using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using Sonosthesia.Touch.Providers;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Link
{
    /// <summary>
    /// Converting straight to MIDI without going through touch allows easier configuration
    /// Consider the general case of this, when we need ways to attach additional data to the
    /// touch (here midi note, channel or whatnot), how should we proceed ?
    /// </summary>
    
    public class CollisionMIDINoteDriver : CollisionChannelDriver<MIDINote>
    {
        [SerializeField] private CollisionValueProvider<float> _channelProvider;
        [SerializeField] private CollisionValueProvider<float> _noteProvider;
        [SerializeField] private CollisionValueProvider<float> _velocityProvider;
        
        protected override bool MakePayload(Collision collision, TransformDynamics dynamics, out MIDINote payload)
        {
            int channel = _channelProvider.GetIntValue(collision, dynamics, 0, 15);
            int note = _noteProvider.GetIntValue(collision, dynamics, 0, 127);
            int velocity = _velocityProvider.GetIntValue(collision, dynamics, 0, 127);
            payload = new MIDINote(channel, note, velocity);
            return true;
        }
    }
}