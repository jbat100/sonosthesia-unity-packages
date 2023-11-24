using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public abstract class MIDIOutput : MonoBehaviour, IMIDIBroadcaster
    {
        public abstract void BroadcastNoteOn(MIDINote note);
        public abstract void BroadcastNoteOff(MIDINote note);
        public abstract void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch);
        public abstract void BroadcastControl(MIDIControl control);
        public abstract void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch);
        public abstract void BroadcastPitchBend(MIDIPitchBend pitchBend);
        public abstract void BroadcastClock(MIDIClock clock);
        public abstract void BroadcastPositionPointer(MIDISongPositionPointer pointer);
        public abstract void BroadcastSync(MIDISync sync);
    }    
}

