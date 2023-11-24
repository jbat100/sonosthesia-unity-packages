using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIClock
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("count")]
        public int Count { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedMIDIClock)} {Port} {Count}";
        }
    }

    public static class PackedMIDIClockExtensions
    {
        public static MIDIClock Unpack(this PackedMIDIClock clock)
        {
            return new MIDIClock(clock.Count);
        }
    }
}