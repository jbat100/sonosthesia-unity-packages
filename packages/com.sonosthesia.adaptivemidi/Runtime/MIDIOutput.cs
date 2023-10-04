using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public abstract class MIDIOutput : MonoBehaviour
    {
        public abstract void BroadcastNoteOn(MIDINote note);
        public abstract void BroadcastNoteOff(MIDINote note);
        public abstract void BroadcastControl(MIDIControl control);
        public abstract void BroadcastAftertouch(MIDIPolyphonicAftertouch aftertouch);
    }    
}

