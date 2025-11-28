using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDIControlSink : Target<MIDIControl>
    {
        [SerializeField] private InterfaceReference<IMIDIMessageBroadcaster> _output;
        
        protected override void Apply(MIDIControl value)
        {
            _output.Value.Broadcast(value);
        }
    }
}