using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIScaleNoteFilter : MIDINoteFilter
    {
        private static readonly MIDIScaleProvider _provider = new MIDIScaleProvider();

        [SerializeField] private ScaleDescriptor _scale;
        public ScaleDescriptor Scale
        {
            get => _scale;
            set => BroadcastSet(value, ref _scale);
        }

        [SerializeField] private MIDINoteName _note;
        public MIDINoteName Note
        {
            get => _note;
            set => BroadcastSet(value, ref _note);
        }
        
        public override bool Allow(int note)
        {
            MIDIScale scale = _provider.GetScale(_scale);
            return scale != null && scale.ContainsNote(_note, (MIDIPitch)note);
        }
    }
}