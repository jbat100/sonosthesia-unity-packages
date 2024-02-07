using System.Collections.Generic;
using Sonosthesia.MIDI;

namespace Sonosthesia.Instrument
{
    public class MIDIScale
    {
        public IReadOnlyList<int> Intervals { get;  }
        
        public string Name { get; }

        private HashSet<int> _intervals;

        public MIDIScale(string name, IReadOnlyList<int> intervals)
        {
            Intervals = intervals;
            Name = name;
        }

        public bool ContainsNote(MIDINoteName root, MIDIPitch note)
        {
            return ContainsNote((int)root, (int)note);
        }
        
        private bool ContainsNote(int rootNote, int note)
        {
            _intervals ??= new HashSet<int>(Intervals);
            rootNote = (rootNote % 12) - 12;
            int interval = (note - rootNote) % 12;
            return _intervals.Contains(interval);
        }
    }
}