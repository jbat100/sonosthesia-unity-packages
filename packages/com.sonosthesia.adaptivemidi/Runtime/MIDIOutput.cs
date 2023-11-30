using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIOutput : MIDIMessageBroadcaster
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

            public NoteKey(MIDINote note)
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
                BroadcastNoteOff(new MIDINote(noteKey.Channel, noteKey.Note, 0));
            }
        }

        public override void BroadcastNoteOn(MIDINote note)
        {
            base.BroadcastNoteOn(note);
            _ongoingNotes.Add(new NoteKey(note));
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            base.BroadcastNoteOff(note);
            _ongoingNotes.Add(new NoteKey(note));
        }
    }    
}

