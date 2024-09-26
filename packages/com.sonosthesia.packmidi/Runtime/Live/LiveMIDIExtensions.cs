using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Pack;

namespace Sonosthesia.PackMIDI
{
    internal static class PackLiveMIDIAddress
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
    
    public static class LiveMIDIExtensions
    {
        public static bool Check(this IPackedLiveMIDIMessage message, string track)
        {
            return string.IsNullOrEmpty(track) || message.Track == track;
        }
        
        public static MIDIPolyphonicAftertouch Unpack(this PackedLiveMIDIPolyphonicAftertouch note)
        {
            return new MIDIPolyphonicAftertouch(note.Channel, note.Note, note.Value);
        }

        public static MIDIPitchBend Unpack(this PackedLiveMIDIPitchBend note)
        {
            return new MIDIPitchBend(note.Channel, note.Value);
        }

        public static MIDINoteOn UnpackNoteOn(this PackedLiveMIDINote note)
        {
            return new MIDINoteOn(note.Channel, note.Note, note.Velocity);
        }
        
        public static MIDINoteOff UnpackNoteOff(this PackedLiveMIDINote note)
        {
            return new MIDINoteOff(note.Channel, note.Note, note.Velocity);
        }

        public static MIDIControl Unpack(this PackedLiveMIDIControl control)
        {
            return new MIDIControl(control.Channel, control.Number, control.Value);
        }

        public static MIDIChannelAftertouch Unpack(this PackedLiveMIDIChannelAftertouch note)
        {
            return new MIDIChannelAftertouch(note.Channel, note.Value);
        }
    }
}