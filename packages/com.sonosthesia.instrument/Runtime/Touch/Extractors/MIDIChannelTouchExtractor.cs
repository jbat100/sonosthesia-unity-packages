using Sonosthesia.Touch;
using UnityEngine;


namespace Sonosthesia.Instrument
{
    public abstract class MIDIChannelTouchExtractor : ScriptableObject
    {
        public abstract bool TryExtract(TouchEvent touchEvent, out int channel);
    }
}