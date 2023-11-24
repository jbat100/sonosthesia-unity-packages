using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMIDIOutput : MIDIOutput
    {
        [SerializeField] private AddressedPackConnection _connection;
        
        [SerializeField] private string _port;

        protected virtual void Awake()
        {
            if (!_connection)
            {
                _connection = GetComponentInParent<AddressedPackConnection>();
            }
        }

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

        public override void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            PackedMIDIPolyphonicAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.POLYPHONIC_AFTERTOUCH, packed);
        }

        public override void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch)
        {
            PackedMIDIChannelAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.CHANNEL_AFTERTOUCH, packed);
        }

        public override void BroadcastPitchBend(MIDIPitchBend pitchBend)
        {
            PackedMIDIPitchBend packed = pitchBend.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.PITCH_BEND, packed);
        }

        public override void BroadcastClock(MIDIClock clock)
        {
            throw new System.NotImplementedException();
        }

        public override void BroadcastPositionPointer(MIDISongPositionPointer pointer)
        {
            throw new System.NotImplementedException();
        }

        public override void BroadcastSync(MIDISync sync)
        {
            throw new System.NotImplementedException();
        }
    }
}