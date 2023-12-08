using Sonosthesia.AdaptiveMIDI.Extensions;
using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Examples
{
    public class MIDIOutputExample : MonoBehaviour
    {
        [SerializeField] private MIDIOutput _output;

        public void ExplicitAPI()
        {
            _output.Broadcast(new MIDINoteOn(1, 60, 80));
            _output.Broadcast(new MIDINoteOff(1, 60, 10));
            _output.Broadcast(new MIDIControl(1, 74, 30));
            _output.Broadcast(new MIDIChannelAftertouch(1, 40));
            _output.Broadcast(new MIDIPolyphonicAftertouch(1, 60, 80));
            _output.Broadcast(new MIDIPitchBend(1, 0.3f, 48f));
        }
        
        public void ConvenienceAPI()
        {
            _output.BroadcastNoteOn(1, 60, 80);
            _output.BroadcastNoteOff(1, 60, 10);
            _output.BroadcastControl(1, 74, 30);
            _output.BroadcastChannelAftertouch(1, 40);
            _output.BroadcastPolyphonicAftertouch(1, 60, 80);
            _output.BroadcastPitchBend(1, 0.3f, 48f);
        }
    }
}