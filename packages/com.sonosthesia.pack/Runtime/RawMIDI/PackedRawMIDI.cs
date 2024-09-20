using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    public interface IPackedRawMIDISourceData
    {
        string Port { get; }
        
        double DeltaTime { get; }
        
        double CumulativeTime { get; }
    }

    public static class PackedRawMIDISourceDataExtensions
    {
        public static TimeSpan Timestamp(this IPackedRawMIDISourceData data)
        {
            return TimeSpan.FromSeconds(data.CumulativeTime);
        }
    }
    
    public interface IPackedRawMIDISinkData
    {
        string Port { get; }
    }

    [MessagePackObject]
    public class PackedRawMIDISinkSingle : IPackedRawMIDISinkData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedRawMIDISinkDouble : IPackedRawMIDISinkData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("b0")]
        public int B0 { get; set; }
        
        [Key("b1")]
        public int B1 { get; set; }
    }
    
    [MessagePackObject]
    public class PackedRawMIDISinkTripple : IPackedRawMIDISinkData
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
    public class PackedRawMIDISourceSingle : IPackedRawMIDISourceData
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
    public class PackedRawMIDISourceDouble : IPackedRawMIDISourceData
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
    public class PackedRawMIDISourceTripple : IPackedRawMIDISourceData
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