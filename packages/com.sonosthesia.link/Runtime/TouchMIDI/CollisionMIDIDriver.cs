using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Link
{
    /// <summary>
    /// Converting straight to MIDI without going through touch allows easier configuration
    /// Consider the general case of this, when we need ways to attach additional data to the
    /// touch (here midi note, channel or whatnot), how should we proceed ?
    /// </summary>
    
    public class CollisionMIDIDriver : CollisionChannelDriver<MIDINote>
    {
        protected override bool MakePayload(Collision collision, out MIDINote payload)
        {
            
        }
    }
}