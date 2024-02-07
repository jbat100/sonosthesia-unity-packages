using System.Collections.Generic;
using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDINameNoteFilter : MIDINoteFilter
    {
        [SerializeField] private List<MIDINoteName> _notes;
        
        public override bool Allow(int note)
        {
            return _notes.Contains(note.ToMIDINoteName());
        }
    }
}