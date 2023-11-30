using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class RawMIDIOutput : MIDIOutput
    {
        private readonly MIDIEncoder _encoder = new();
        [SerializeField] private RawMIDIOutputStream _rawOutput;

        protected virtual void Awake()
        {
            if (!_rawOutput)
            {
                _rawOutput = GetComponent<RawMIDIOutputStream>();
            }
        }
        
        public override void BroadcastNoteOn(MIDINote note)
        {
            base.BroadcastNoteOn(note);
            _encoder.EncodeNoteOn(_rawOutput, note);
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            base.BroadcastNoteOff(note);
            _encoder.EncodeNoteOff(_rawOutput, note);
        }

        public override void BroadcastControl(MIDIControl control)
        {
            base.BroadcastControl(control);
            _encoder.EncodeControl(_rawOutput, control);
        }

        public override void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            base.BroadcastPolyphonicAftertouch(aftertouch);
            _encoder.EncodePolyphonicAftertouch(_rawOutput, aftertouch);
        }

        public override void BroadcastPitchBend(MIDIPitchBend pitchBend)
        {
            base.BroadcastPitchBend(pitchBend);
            _encoder.EncodePitchBend(_rawOutput, pitchBend);
        }

        public override void BroadcastClock(MIDIClock clock)
        {
            base.BroadcastClock(clock);
            _encoder.EncodeClock(_rawOutput, clock);
        }

        public override void BroadcastPositionPointer(MIDISongPositionPointer pointer)
        {
            base.BroadcastPositionPointer(pointer);
            _encoder.EncodePositionPointer(_rawOutput, pointer);
        }

        public override void BroadcastSync(MIDISync sync)
        {
            base.BroadcastSync(sync);
            _encoder.EncodeSync(_rawOutput, sync);
        }

        public override void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch)
        {
            base.BroadcastChannelAftertouch(aftertouch);
            _encoder.EncodeChannelAftertouch(_rawOutput, aftertouch);
        }
    }
}