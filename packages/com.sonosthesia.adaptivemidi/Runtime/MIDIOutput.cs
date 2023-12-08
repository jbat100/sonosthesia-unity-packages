using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIOutput : MIDIMessageNode
    {
        private readonly struct NoteKey
        {
            public readonly int Channel;
            public readonly int Note;

            public NoteKey(int channel, int note)
            {
                Channel = channel;
                Note = note;
            }

            public NoteKey(MIDINoteOn note)
            {
                Channel = note.Channel;
                Note = note.Note;
            }
            
            public NoteKey(MIDINoteOff note)
            {
                Channel = note.Channel;
                Note = note.Note;
            }
        }

        private readonly HashSet<NoteKey> _ongoingNotes = new();

        public void ClearOngoingNotes()
        {
            foreach (NoteKey noteKey in _ongoingNotes)
            {
                Broadcast(new MIDINoteOff(noteKey.Channel, noteKey.Note, 0));
            }
        }

        public override void Broadcast(MIDINoteOn note)
        {
            base.Broadcast(note);
            _ongoingNotes.Add(new NoteKey(note));
        }

        public override void Broadcast(MIDINoteOff note)
        {
            base.Broadcast(note);
            _ongoingNotes.Remove(new NoteKey(note));
        }
    }    
}

