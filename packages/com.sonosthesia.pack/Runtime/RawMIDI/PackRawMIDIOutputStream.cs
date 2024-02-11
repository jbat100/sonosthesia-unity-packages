using Sonosthesia.AdaptiveMIDI;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackRawMIDIOutputStream : RawMIDIOutputStream
    {
        [SerializeField] private PackEnvelopeHub envelopeHub;
        
        [SerializeField] private string _port;

        public override void Broadcast(byte data0)
        {
            PackedRawMIDISinkSingle data = new PackedRawMIDISinkSingle
            {
                Port = _port,
                B0 = data0
            };
            envelopeHub.QueueOutgoingContent(PackRawMIDISinkAddress.SINGLE, data);
        }

        public override void Broadcast(byte data0, byte data1)
        {
            PackedRawMIDISinkDouble data = new PackedRawMIDISinkDouble
            {
                Port = _port,
                B0 = data0,
                B1 = data1
            };
            envelopeHub.QueueOutgoingContent(PackRawMIDISinkAddress.DOUBLE, data);
        }

        public override void Broadcast(byte data0, byte data1, byte data2)
        {
            PackedRawMIDISinkTripple data = new PackedRawMIDISinkTripple
            {
                Port = _port,
                B0 = data0,
                B1 = data1,
                B2 = data2
            };
            envelopeHub.QueueOutgoingContent(PackRawMIDISinkAddress.TRIPPLE, data);
        }
    }
}