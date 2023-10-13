namespace Sonosthesia.Pack
{
    public interface IPackedAddressedMIDIMessage 
    {
        string Port { get; }
        
        string Track { get; }
    }

    public static class PackedMIDIPortMessageExtensions
    {
        public static bool Check(this IPackedAddressedMIDIMessage message, string port, string track)
        {
            return (string.IsNullOrEmpty(port) || message.Port == port) &&
                   (string.IsNullOrEmpty(track) || message.Track == track);
        }
    }
}