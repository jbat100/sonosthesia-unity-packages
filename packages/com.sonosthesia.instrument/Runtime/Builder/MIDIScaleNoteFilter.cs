using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIScaleNoteFilter : MIDINoteFilter
    {
        private static readonly MIDIScaleProvider _provider = new MIDIScaleProvider();  
        
        [SerializeField] private ScaleDescriptor _scale;

        [SerializeField] private MIDINoteName _note;
        
        public override bool Allow(int note)
        {
            MIDIScale scale = _provider.GetScale(_scale);
            if (scale == null)
            {
                return false;
            }

            return scale.ContainsNote(_note, (MIDIPitch)note);
        }
    }
}