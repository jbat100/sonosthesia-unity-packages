using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackLiveMIDIOutput : MIDIOutput
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
            PackedLiveMIDINote packedLive = note.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.NOTE_ON, packedLive);
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            base.BroadcastNoteOff(note);
            PackedLiveMIDINote packedLive = note.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.NOTE_OFF, packedLive);
        }

        public override void BroadcastControl(MIDIControl control)
        {
            base.BroadcastControl(control);
            PackedLiveMIDIControl packed = control.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.CONTROL, packed);
        }

        public override void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            base.BroadcastPolyphonicAftertouch(aftertouch);
            PackedLiveMIDIPolyphonicAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.POLYPHONIC_AFTERTOUCH, packed);
        }

        public override void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch)
        {
            base.BroadcastChannelAftertouch(aftertouch);
            PackedLiveMIDIChannelAftertouch packed = aftertouch.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.CHANNEL_AFTERTOUCH, packed);
        }

        public override void BroadcastPitchBend(MIDIPitchBend pitchBend)
        {
            base.BroadcastPitchBend(pitchBend);
            PackedLiveMIDIPitchBend packed = pitchBend.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.PITCH_BEND, packed);
        }

        public override void BroadcastClock(MIDIClock clock)
        {
            base.BroadcastClock(clock);
            PackedLiveMIDIClock packed = clock.Pack(_port);
            _connection.QueueOutgoingContent(PackLiveMIDIAddress.CLOCK, packed);
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