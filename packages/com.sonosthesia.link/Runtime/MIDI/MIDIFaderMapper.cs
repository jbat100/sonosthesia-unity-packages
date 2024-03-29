using UnityEngine;
using Sonosthesia.MIDI;

namespace Sonosthesia.Link
{
    public abstract class MIDIFaderMapper<TTarget> : FaderLinkMapper<MIDINote, TTarget> where TTarget : struct
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
        
        // TODO : seems like this is overly complicated something which could be done with a selector 
        
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


