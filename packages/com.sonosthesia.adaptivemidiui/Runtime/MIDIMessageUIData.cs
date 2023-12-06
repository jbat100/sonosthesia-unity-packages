using System;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDIUI
{
    public readonly struct MIDIMessageUIData
    {
        public readonly int Count;
        public readonly string Type;
        public readonly string Data;
        public readonly TimeSpan Timestamp;

        public MIDIMessageUIData(int count, string type, string data, TimeSpan timestamp)
        {
            Count = count;
            Type = type;
            Data = data;
            Timestamp = timestamp;
        }
    }
    
    static class MIDIMessageUIExtensions
    {
        private static MIDIMessageUIData FullDescription(string messageDescription)
        {
            return default;
        }
        
        public static MIDIMessageUIData UIData(this MIDIClock clock, int count)
        {
            return new MIDIMessageUIData(count, "clock", $"<tick {clock.Count}>", clock.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDISync sync, int count)
        {
            return new MIDIMessageUIData(count, "sync", $"<{sync.Type}>", sync.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDISongPositionPointer pointer, int count)
        {
            return new MIDIMessageUIData(count, "position", $"<beats {pointer.Position}>", pointer.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDINote note, bool on, int count)
        {
            return new MIDIMessageUIData(count, $"note-{(on ? "on" : "off")}", $"<chan {note.Channel} pitch {note.Note} vel {note.Velocity}>", note.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDIPolyphonicAftertouch aftertouch, int count)
        {
            return new MIDIMessageUIData(count, "poly-aftertouch", $"<chan {aftertouch.Channel} pitch {aftertouch.Note} val {aftertouch.Value}>", aftertouch.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDIControl control, int count)
        {
            return new MIDIMessageUIData(count, "control", $"<chan {control.Channel} num {control.Number} val {control.Value}>", control.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDIChannelAftertouch aftertouch, int count)
        {
            return new MIDIMessageUIData(count, "chan-aftertouch", $"<chan {aftertouch.Channel} val {aftertouch.Value}>", aftertouch.Timestamp);
        }
        
        public static MIDIMessageUIData UIData(this MIDIPitchBend bend, int count)
        {
            return new MIDIMessageUIData(count, "pitch-bend", $"<chan {bend.Channel} val {bend.Value}>", bend.Timestamp);
        }
    }
}