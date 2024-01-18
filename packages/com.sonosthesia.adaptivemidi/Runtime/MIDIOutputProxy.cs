using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIOutputProxy : MIDIOutput
    {
        [SerializeField] private MIDIOutput _output;

        public override void Broadcast(MIDINoteOn note)
        {
            base.Broadcast(note);
            if (_output)
            {
                _output.Broadcast(note);   
            }
        }
        
        public override void Broadcast(MIDINoteOff note)
        {
            base.Broadcast(note);
            if (_output)
            {
                _output.Broadcast(note);   
            }
        }

        public override void Broadcast(MIDIControl control)
        {
            base.Broadcast(control);
            if (_output)
            {
                _output.Broadcast(control);   
            }
        }

        public override void Broadcast(MIDIChannelAftertouch aftertouch)
        {
            base.Broadcast(aftertouch);
            if (_output)
            {
                _output.Broadcast(aftertouch);   
            }
        }

        public override void Broadcast(MIDIPolyphonicAftertouch aftertouch)
        {
            base.Broadcast(aftertouch);
            if (_output)
            {
                _output.Broadcast(aftertouch);   
            }
        }

        public override void Broadcast(MIDIPitchBend pitchBend)
        {
            base.Broadcast(pitchBend);
            if (_output)
            {
                _output.Broadcast(pitchBend);   
            }
        }

        public override void Broadcast(MIDIClock clock)
        {
            base.Broadcast(clock);
            if (_output)
            {
                _output.Broadcast(clock);   
            }
        }

        public override void Broadcast(MIDISongPositionPointer pointer)
        {
            base.Broadcast(pointer);
            if (_output)
            {
                _output.Broadcast(pointer);   
            }
        }

        public override void Broadcast(MIDISync sync)
        {
            base.Broadcast(sync);
            if (_output)
            {
                _output.Broadcast(sync);   
            }
        }
    }
}