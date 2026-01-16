using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIStaticPitchedElement : MonoBehaviour, IMIDIPitchProvider
    {
        [SerializeField] private int _midiNote;
        
        public int MIDIPitch 
        {
            get => _midiNote;
            set => _midiNote = value;
        }
        
    }
}