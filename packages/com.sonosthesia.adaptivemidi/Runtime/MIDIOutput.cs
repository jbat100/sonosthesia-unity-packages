using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public abstract class MIDIOutput : MonoBehaviour
    {
        protected abstract void BroadcastNoteOn(MIDINote note);
        protected abstract void BroadcastNoteOff(MIDINote note);
        protected abstract void BroadcastControl(MIDIControl control);
        protected abstract void BroadcastAftertouch(MIDIPolyphonicAftertouch aftertouch);
    }    
}

