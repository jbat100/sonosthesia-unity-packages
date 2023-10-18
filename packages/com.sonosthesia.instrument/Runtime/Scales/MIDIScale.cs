using System.Collections.Generic;

namespace Sonosthesia.Instrument
{
    public class MIDIScale
    {
        public IReadOnlyList<int> Intervals { get;  }
        
        public string Name { get; }

        public MIDIScale(string name, IReadOnlyList<int> intervals)
        {
            Intervals = intervals;
            Name = name;
        }
    }
}