using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public abstract class MIDIFaderMapper<TTarget> : FaderMapper<MIDINote, TTarget> where TTarget : struct
    {
        private enum Driver
        {
            None,
            Channel,
            Note,
            Velocity,
            Random
        }
        
        [SerializeField] private Driver _driver;
        
        protected override float Drive(MIDINote payload)
        {
            return _driver switch
            {
                Driver.Channel => payload.Channel / 15f,
                Driver.Note => payload.Note / 127f,
                Driver.Velocity => payload.Velocity,
                Driver.Random => Random.value,
                _ => 0f
            };
        }
    }    
}


