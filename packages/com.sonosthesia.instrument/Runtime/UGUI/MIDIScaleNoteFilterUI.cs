using System.Collections.Generic;
using Sonosthesia.MIDI;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia.Instrument
{
    public class MIDIScaleNoteFilterUI : MonoBehaviour
    {
        private readonly BijectionDictionary<MIDINoteName, string> _notes =
            new (new Dictionary<MIDINoteName, string>()
            {
                { MIDINoteName.C, "C"},
                { MIDINoteName.CSharp, "C#"},
                { MIDINoteName.D, "D"},
                { MIDINoteName.DSharp, "D#"},
                { MIDINoteName.E, "E"},
                { MIDINoteName.F, "F"},
                { MIDINoteName.FSharp, "F#"},
                { MIDINoteName.G, "G"},
                { MIDINoteName.GSharp, "G#"},
                { MIDINoteName.A, "A"},
                { MIDINoteName.ASharp, "A#"},
                { MIDINoteName.B, "B"}
            });
        
        private readonly BijectionDictionary<ScaleDescriptor, string> _scales =
            new (new Dictionary<ScaleDescriptor, string>()
            {
                { ScaleDescriptor.All, "All" },
                { ScaleDescriptor.Ionian, "Ionian" },
                { ScaleDescriptor.Aeolian, "Aeolian" },
                { ScaleDescriptor.HarmonicMinor, "Harmonic Minor" },
                { ScaleDescriptor.MelodicMinor, "Melodic Minor" },
                { ScaleDescriptor.Dorian, "Dorian" },
                { ScaleDescriptor.Phrygian, "Phrygian" },
                { ScaleDescriptor.Lydian, "Lydian" },
                { ScaleDescriptor.Mixolydian, "Mixolydian" },
                { ScaleDescriptor.Locrian, "Locrian" },
                { ScaleDescriptor.WholeTone, "Whole Tone" },
                { ScaleDescriptor.PentatonicMajor, "Pentatonic Major" },
                { ScaleDescriptor.PentatonicMinor, "Pentatonic Minor" },
                { ScaleDescriptor.Blues, "Blues" },
                { ScaleDescriptor.DiminishedWhole, "Diminished Whole" },
                { ScaleDescriptor.DiminishedHalf, "Diminished Half" }
            });

        [SerializeField] private Dropdown _noteDropdown;

        [SerializeField] private Dropdown _scaleDropdown;

        [SerializeField] private MIDIScaleNoteFilter _filter;

        private readonly CompositeDisposable _bindings = new ();

        protected virtual void OnEnable()
        {
            _bindings.Clear();
            
            _bindings.Add(new DropdownBinding<MIDINoteName>(_noteDropdown, _notes, _filter.ChangeObservable,
                () => _filter.Note, note => _filter.Note = note));
            
            _bindings.Add(new DropdownBinding<ScaleDescriptor>(_scaleDropdown, _scales, _filter.ChangeObservable,
                () => _filter.Scale, scale => _filter.Scale = scale));
        }
        
        protected virtual void OnDisable()
        {
            _bindings.Clear();
        }
    }
}