namespace Sonosthesia.Pack
{
    public interface IPackedMIDIPortMessage 
    {
        string Port { get; }
        
        string Track { get; }
    }
}