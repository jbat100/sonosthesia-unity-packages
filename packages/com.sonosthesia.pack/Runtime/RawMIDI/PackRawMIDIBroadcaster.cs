using Sonosthesia.AdaptiveMIDI;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackRawMIDIBroadcaster : RawMIDIBroadcaster
    {
        [SerializeField] private AddressedPackConnection _connection;
        
        [SerializeField] private string _port;

        public override void Broadcast(byte data0)
        {
            PackedMIDIRawSinkSingle data = new PackedMIDIRawSinkSingle
            {
                Port = _port,
                B0 = data0
            };
            _connection.QueueOutgoingContent(PackMIDIRawSinkAddress.SINGLE, data);
        }

        public override void Broadcast(byte data0, byte data1)
        {
            PackedMIDIRawSinkDouble data = new PackedMIDIRawSinkDouble
            {
                Port = _port,
                B0 = data0,
                B1 = data1
            };
            _connection.QueueOutgoingContent(PackMIDIRawSinkAddress.DOUBLE, data);
        }

        public override void Broadcast(byte data0, byte data1, byte data2)
        {
            PackedMIDIRawSinkTripple data = new PackedMIDIRawSinkTripple
            {
                Port = _port,
                B0 = data0,
                B1 = data1,
                B2 = data2
            };
            _connection.QueueOutgoingContent(PackMIDIRawSinkAddress.TRIPPLE, data);
        }
    }
}