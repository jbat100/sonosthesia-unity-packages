using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public abstract class RawMIDIOutputStream : MonoBehaviour, IRawMIDIBroadcaster
    {
        public abstract void Broadcast(byte data0);
        public abstract void Broadcast(byte data0, byte data1);
        public abstract void Broadcast(byte data0, byte data1, byte data2);
    }
}