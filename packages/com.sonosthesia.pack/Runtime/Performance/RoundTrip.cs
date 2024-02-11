using MessagePack;

namespace Sonosthesia.Pack
{
    internal static class PackPerformanceAddress
    {
        public const string ROUNDTRIP_REQUEST = "/roundtrip/request";
        public const string ROUNDTRIP_RESPONSE = "/roundtrip/response";
    }
    
    [MessagePackObject]
    public class RoundTripRequest
    {
        [Key("id")] public string Id { get; set; }
        
        [Key("timestamp")] public float Timestamp { get; set; }
    }
    
    [MessagePackObject]
    public class RoundTripResponse
    {
        [Key("id")] public string Id { get; set; }
        
        [Key("timestamp")] public float Timestamp { get; set; }
    }
}