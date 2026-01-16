using UnityEngine;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.MIDI
{
    public class MIDIControlFloatSink : Target<float>
    {
        [SerializeField] private MIDIOutput _output;

        [SerializeField] private int _channel;

        [SerializeField] private int _number;

        protected override void Apply(float value)
        {
            MIDIControl control = new MIDIControl(_channel, _number, (int)value);
            this.LogVerbose($"{this} Broadcast {control}");
            _output.Broadcast(control);
        }
    }
}

