using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class Counting
    {
        [Key("counter")]
        public int Counter { get; set; }
        
        [Key("from")]
        public string From { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Counter)} {Counter} {nameof(From)} {From}";
        }
    }
}