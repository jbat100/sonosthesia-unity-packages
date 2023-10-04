using Cysharp.Threading.Tasks;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMIDIOutput : MIDIOutput
    {
        [SerializeField] private AddressedPackConnection _connection;
        
        [SerializeField] private string _port;

        public override void BroadcastNoteOn(MIDINote note)
        {
            PackedMIDINote packed = note.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.NOTE_ON, packed);
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            PackedMIDINote packed = note.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.NOTE_OFF, packed);
        }

        public override void BroadcastControl(MIDIControl control)
        {
            PackedMIDIControl packed = control.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.CONTROL, packed);
        }

        public override void BroadcastAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            PackedMIDIPolyphonicAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.AFTERTOUCH, packed);
        }
    }
}