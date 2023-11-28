using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    [RequireComponent(typeof(RawMIDIBroadcaster))]
    public class RawMIDIOutput : MIDIOutput
    {
        private readonly MIDIEncoder _encoder = new();
        private RawMIDIBroadcaster _broadcaster;

        protected virtual void Awake()
        {
            _broadcaster = GetComponent<RawMIDIBroadcaster>();
        }
        
        public override void BroadcastNoteOn(MIDINote note)
        {
            _encoder.EncodeNoteOn(_broadcaster, note);
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            _encoder.EncodeNoteOff(_broadcaster, note);
        }

        public override void BroadcastControl(MIDIControl control)
        {
            _encoder.EncodeControl(_broadcaster, control);
        }

        public override void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            _encoder.EncodePolyphonicAftertouch(_broadcaster, aftertouch);
        }

        public override void BroadcastPitchBend(MIDIPitchBend pitchBend)
        {
            _encoder.EncodePitchBend(_broadcaster, pitchBend);
        }

        public override void BroadcastClock(MIDIClock clock)
        {
            _encoder.EncodeClock(_broadcaster, clock);
        }

        public override void BroadcastPositionPointer(MIDISongPositionPointer pointer)
        {
            _encoder.EncodePositionPointer(_broadcaster, pointer);
        }

        public override void BroadcastSync(MIDISync sync)
        {
            _encoder.EncodeSync(_broadcaster, sync);
        }

        public override void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch)
        {
            _encoder.EncodeChannelAftertouch(_broadcaster, aftertouch);
        }
    }
}