using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public abstract class MIDIOutput : MonoBehaviour
    {
        protected abstract void Broadcast(MIDINote note);
        protected abstract void Broadcast(MIDIControl control);
        protected abstract void Broadcast(MIDIPolyphonicAftertouch aftertouch);
    }    
}

