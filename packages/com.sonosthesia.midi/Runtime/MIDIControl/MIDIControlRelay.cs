using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    [CreateAssetMenu(fileName = "MIDIControlRelay", menuName = "Sonosthesia/Relays/MIDIControlRelay")]
    public class MIDIControlRelay : StatelessSignalRelay<MIDIControl>
    {
        
    }
}