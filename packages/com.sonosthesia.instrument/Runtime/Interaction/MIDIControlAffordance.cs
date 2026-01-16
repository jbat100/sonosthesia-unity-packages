using System;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIControlAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct
    {
        [SerializeField] private InterfaceReference<IMIDIControlConfiguration<TEvent>> _configuration;

        [SerializeField] private InterfaceReference<IMIDIMessageBroadcaster> _output;

        private TriggerImplementation _trigger;
        
        private class Controller : AffordanceController<TEvent, MIDIControlAffordance<TEvent>> 
        {
            public Controller(Guid eventId, MIDIControlAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }
        }
    }
}