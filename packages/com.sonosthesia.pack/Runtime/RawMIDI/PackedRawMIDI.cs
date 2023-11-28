using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    internal static class PackMIDIRawSourceAddress
    {
        public const string SINGLE                  = "/midi/source/single";
        public const string DOUBLE                  = "/midi/source/double";
        public const string TRIPPLE                 = "/midi/source/tripple";
    }
    
    internal static class PackMIDIRawSinkAddress
    {
        public const string SINGLE                  = "/midi/sink/single";
        public const string DOUBLE                  = "/midi/sink/double";
        public const string TRIPPLE                 = "/midi/sink/tripple";
    }
    
    public interface IPackedMIDIRawSourceData
    {
        string Port { get; }
        
        double DeltaTime { get; }
        
        double CumulativeTime { get; }
    }

    public static class PackedMIDIRawSourceDataExtensions
    {
        public static TimeSpan Timestamp(this IPackedMIDIRawSourceData data)
        {
            return TimeSpan.FromSeconds(data.CumulativeTime);
        }
    }
    
    public interface IPackedMIDIRawSinkData
    {
        string Port { get; }
    }

    [MessagePackObject]
    public class PackedMIDIRawSinkSingle : IPackedMIDIRawSinkData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedMIDIRawSinkDouble : IPackedMIDIRawSinkData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("b0")]
        public int B0 { get; set; }
        
        [Key("b1")]
        public int B1 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedMIDIRawSinkTripple : IPackedMIDIRawSinkData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
        
        [Key("b1")]
        public byte B1 { get; set; }
        
        [Key("b2")]
        public byte B2 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedMIDIRawSourceSingle : IPackedMIDIRawSourceData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("deltaTime")]
        public double DeltaTime { get; set; }
        
        [Key("cumulativeTime")]
        public double CumulativeTime { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedMIDIRawSourceDouble : IPackedMIDIRawSourceData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("deltaTime")]
        public double DeltaTime { get; set; }
        
        [Key("cumulativeTime")]
        public double CumulativeTime { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
        
        [Key("b1")]
        public byte B1 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedMIDIRawSourceTripple : IPackedMIDIRawSourceData
    {
        [Key("port")]
        public string Port { get; set; }

        [Key("deltaTime")]
        public double DeltaTime { get; set; }
        
        [Key("cumulativeTime")]
        public double CumulativeTime { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
        
        [Key("b1")]
        public byte B1 { get; set; }
        
        [Key("b2")]
        public byte B2 { get; set; }
    }
}