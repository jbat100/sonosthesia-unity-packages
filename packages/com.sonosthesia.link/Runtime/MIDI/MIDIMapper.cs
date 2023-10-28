using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Mapping;

namespace Sonosthesia.Link
{
    public abstract class MIDIMapper<T> : LinkMapper<MIDINote, T> where T : struct { }
}


