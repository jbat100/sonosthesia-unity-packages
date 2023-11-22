using Sonosthesia.AdaptiveMIDI;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Metronome
{
    public class MIDIBeatSignalDriver : MonoBehaviour
    {
        [SerializeField] private Signal<Beat> _signal;

        [SerializeField] private MIDIInput _input;
        
        

    }
}