using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Mapping;

namespace Sonosthesia.MIDI
{
    public abstract class MIDIMapper<T> : Mapper<MIDINote, T> where T : struct { }
}


