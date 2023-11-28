using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public abstract class RawMIDIBroadcaster : MonoBehaviour, IRawMIDIBroadcaster
    {
        public abstract void Broadcast(byte data0);
        public abstract void Broadcast(byte data0, byte data1);
        public abstract void Broadcast(byte data0, byte data1, byte data2);
    }
}