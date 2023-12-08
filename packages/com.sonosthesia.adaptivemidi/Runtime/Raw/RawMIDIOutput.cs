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
        
        public override void Broadcast(MIDINoteOn note)
        {
            base.Broadcast(note);
            _encoder.EncodeNoteOn(_rawOutput, note);
        }

        public override void Broadcast(MIDINoteOff note)
        {
            base.Broadcast(note);
            _encoder.EncodeNoteOff(_rawOutput, note);
        }

        public override void Broadcast(MIDIControl control)
        {
            base.Broadcast(control);
            _encoder.EncodeControl(_rawOutput, control);
        }

        public override void Broadcast(MIDIPolyphonicAftertouch aftertouch)
        {
            base.Broadcast(aftertouch);
            _encoder.EncodePolyphonicAftertouch(_rawOutput, aftertouch);
        }

        public override void Broadcast(MIDIPitchBend pitchBend)
        {
            base.Broadcast(pitchBend);
            _encoder.EncodePitchBend(_rawOutput, pitchBend);
        }

        public override void Broadcast(MIDIClock clock)
        {
            base.Broadcast(clock);
            _encoder.EncodeClock(_rawOutput, clock);
        }

        public override void Broadcast(MIDISongPositionPointer pointer)
        {
            base.Broadcast(pointer);
            _encoder.EncodePositionPointer(_rawOutput, pointer);
        }

        public override void Broadcast(MIDISync sync)
        {
            base.Broadcast(sync);
            _encoder.EncodeSync(_rawOutput, sync);
        }

        public override void Broadcast(MIDIChannelAftertouch aftertouch)
        {
            base.Broadcast(aftertouch);
            _encoder.EncodeChannelAftertouch(_rawOutput, aftertouch);
        }
    }
}