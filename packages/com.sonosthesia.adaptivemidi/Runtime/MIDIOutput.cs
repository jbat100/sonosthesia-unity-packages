using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    
    public readonly struct ChannelNoteKey : IEquatable<ChannelNoteKey>
    {
        public readonly int Channel;
        public readonly int Note;

        public ChannelNoteKey(int channel, int note)
        {
            Channel = channel;
            Note = note;
        }

        public ChannelNoteKey(MIDINoteOn note)
        {
            Channel = note.Channel;
            Note = note.Note;
        }
            
        public ChannelNoteKey(MIDINoteOff note)
        {
            Channel = note.Channel;
            Note = note.Note;
        }
            
        public ChannelNoteKey(MIDIPolyphonicAftertouch aftertouch)
        {
            Channel = aftertouch.Channel;
            Note = aftertouch.Note;
        }
            
        public bool Equals(ChannelNoteKey other) =>
            Channel == other.Channel && Note == other.Note;

        public override bool Equals(object obj) =>
            obj is ChannelNoteKey other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Channel, Note);

        public static bool operator ==(ChannelNoteKey left, ChannelNoteKey right) => left.Equals(right);
        public static bool operator !=(ChannelNoteKey left, ChannelNoteKey right) => !left.Equals(right);
    }
    
    public class MIDIOutput : MIDIMessageNode
    {
        private readonly HashSet<ChannelNoteKey> _ongoingNotes = new();

        public void ClearOngoingNotes()
        {
            foreach (ChannelNoteKey noteKey in _ongoingNotes)
            {
                Broadcast(new MIDINoteOff(noteKey.Channel, noteKey.Note, 0));
            }
        }
        
        protected virtual void OnDisable()
        {
            ClearOngoingNotes();    
        }

        public override void Broadcast(MIDINoteOn note)
        {
            base.Broadcast(note);
            _ongoingNotes.Add(new ChannelNoteKey(note));
        }

        public override void Broadcast(MIDINoteOff note)
        {
            base.Broadcast(note);
            _ongoingNotes.Remove(new ChannelNoteKey(note));
        }
    }    
}

