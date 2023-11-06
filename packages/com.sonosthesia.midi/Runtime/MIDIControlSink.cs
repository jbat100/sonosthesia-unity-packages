using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDIControlSink : Target<MIDIControl>
    {
        [SerializeField] private MIDIOutput _output;
        
        protected override void Apply(MIDIControl value)
        {
            _output.BroadcastChannelControl(value);
        }
    }
}