namespace Sonosthesia.Pack
{
    internal static class PackMIDIAddress
    {
        public const string NOTE                    = "/midi/note";
        public const string NOTE_ON                 = "/midi/note/on";
        public const string NOTE_OFF                = "/midi/note/off";
        public const string POLYPHONIC_AFTERTOUCH   = "/midi/note/aftertouch";
        public const string CONTROL                 = "/midi/channel/control";
        public const string CHANNEL_AFTERTOUCH      = "/midi/channel/aftertouch";
        public const string PITCH_BEND              = "/midi/channel/bend";
        public const string CLOCK                   = "/midi/clock";
    }
    
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