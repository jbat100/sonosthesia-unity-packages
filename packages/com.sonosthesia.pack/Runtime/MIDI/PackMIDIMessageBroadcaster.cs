using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMIDIMessageBroadcaster : MIDIMessageBroadcaster
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
            base.BroadcastNoteOn(note);
            PackedMIDINote packed = note.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.NOTE_ON, packed);
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            base.BroadcastNoteOff(note);
            PackedMIDINote packed = note.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.NOTE_OFF, packed);
        }

        public override void BroadcastControl(MIDIControl control)
        {
            base.BroadcastControl(control);
            PackedMIDIControl packed = control.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.CONTROL, packed);
        }

        public override void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            base.BroadcastPolyphonicAftertouch(aftertouch);
            PackedMIDIPolyphonicAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.POLYPHONIC_AFTERTOUCH, packed);
        }

        public override void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch)
        {
            base.BroadcastChannelAftertouch(aftertouch);
            PackedMIDIChannelAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.CHANNEL_AFTERTOUCH, packed);
        }

        public override void BroadcastPitchBend(MIDIPitchBend pitchBend)
        {
            base.BroadcastPitchBend(pitchBend);
            PackedMIDIPitchBend packed = pitchBend.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.PITCH_BEND, packed);
        }

        public override void BroadcastClock(MIDIClock clock)
        {
            base.BroadcastClock(clock);
            PackedMIDIClock packed = clock.Pack(_port);
            _connection.QueueOutgoingContent(PackMIDIAddress.CLOCK, packed);
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