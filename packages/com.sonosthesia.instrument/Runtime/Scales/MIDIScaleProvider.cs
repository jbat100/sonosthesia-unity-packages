using System.Collections.Generic;

namespace Sonosthesia.Instrument
{
    public enum ScaleDescriptor
    {
        None,
        Ionian,
        Aeolian,
        HarmonicMinor,
        MelodicMinor,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Locrian,
        WholeTone,
        PentatonicMajor,
        PentatonicMinor,
        Blues,
        DiminishedWhole,
        DiminishedHalf
    }
    
    public class MIDIScaleProvider : IMIDIScaleProvider<ScaleDescriptor>
    {
        private Dictionary<ScaleDescriptor, MIDIScale> _scales = new ()
        {
            {ScaleDescriptor.Ionian, new MIDIScale("Ionian", new []{0, 2, 4, 5, 7, 9, 11})},
            {ScaleDescriptor.Aeolian, new MIDIScale("Aeolian", new []{0, 2, 3, 5, 7, 8, 10})},
            {ScaleDescriptor.HarmonicMinor, new MIDIScale("Ionian", new []{0, 2, 3, 5, 7, 8, 11})},
            {ScaleDescriptor.MelodicMinor, new MIDIScale("Ionian", new []{0, 2, 3, 5, 7, 9, 11})},
            {ScaleDescriptor.Dorian, new MIDIScale("Ionian", new []{0, 2, 3, 5, 7, 9, 10})},
            {ScaleDescriptor.Phrygian, new MIDIScale("Ionian", new []{0, 1, 3, 5, 7, 8, 10})},
            {ScaleDescriptor.Lydian, new MIDIScale("Ionian", new []{0, 2, 4, 6, 7, 9, 11})},
            {ScaleDescriptor.Mixolydian, new MIDIScale("Ionian", new []{0, 2, 4, 5, 7, 9, 10})},
            {ScaleDescriptor.Locrian, new MIDIScale("Ionian", new []{0, 1, 3, 5, 6, 8, 10})},
            {ScaleDescriptor.WholeTone, new MIDIScale("Ionian", new []{0, 2, 4, 6, 8, 10})},
            {ScaleDescriptor.PentatonicMajor, new MIDIScale("Ionian", new []{0, 2, 4, 7, 9})},
            {ScaleDescriptor.PentatonicMinor, new MIDIScale("Ionian", new []{0, 3, 5, 7, 10})},
            {ScaleDescriptor.Blues, new MIDIScale("Ionian", new []{0, 3, 5, 6, 7, 10})},
            {ScaleDescriptor.DiminishedWhole, new MIDIScale("Ionian", new []{0, 2, 3, 5, 6, 8, 9, 11})},
            {ScaleDescriptor.DiminishedHalf, new MIDIScale("Ionian", new []{0, 1, 3, 4, 6, 7, 9, 10})}
        };

        public MIDIScale GetScale(ScaleDescriptor descriptor)
        {
            throw new System.NotImplementedException();
        }
    }
}