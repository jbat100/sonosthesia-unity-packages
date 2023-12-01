using UnityEngine;

namespace Sonosthesia.Timeline.MIDI
{
    // ScriptableObject class used for storing a MIDI file asset
    sealed public class MIDIFileAsset : ScriptableObject
    {
        public MIDIAnimationAsset [] tracks;
    }
}
