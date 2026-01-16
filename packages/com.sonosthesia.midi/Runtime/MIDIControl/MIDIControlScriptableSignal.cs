using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    [CreateAssetMenu(fileName = "MIDIControlSignal", menuName = "Sonosthesia/Signals/MIDIControlSignal")]
    public class MIDIControlScriptableSignal : ScriptableSignal<MIDIControl>
    {
        
    }
}