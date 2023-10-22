using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIStaticPitchedElement : MonoBehaviour, IMIDIPitchedElement
    {
        [SerializeField] private int _midiNote;
        
        public int MIDINote 
        {
            get => _midiNote;
            set => _midiNote = value;
        }
        
    }
}