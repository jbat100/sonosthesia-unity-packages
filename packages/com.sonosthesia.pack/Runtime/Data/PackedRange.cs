using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedRange
    {
        [Key("lower")]
        public float Lower { get; set; }

        [Key("upper")]
        public float Upper { get; set; }
    }
}