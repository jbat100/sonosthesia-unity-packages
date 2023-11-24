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
    
    public interface IPackedMIDIRawSinkData
    {
        string Port { get; }
    }
}