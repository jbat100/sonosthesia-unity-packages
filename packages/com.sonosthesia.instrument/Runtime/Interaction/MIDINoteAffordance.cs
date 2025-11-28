using System;
using Sonosthesia.Extractor;
using Sonosthesia.Interaction;
using Sonosthesia.MIDI;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDINoteAffordance<TEvent> : ChannelAffordance<MIDINote, TEvent> where TEvent : struct
    {
        [SerializeField] private InterfaceReference<IMIDINoteConfiguration<TEvent>> _configuration;

        private class Controller : ChannelAffordanceController<MIDINote, TEvent, MIDINoteAffordance<TEvent>>
        {
            private IDynamicExtractorSession<TEvent, float> _pressure;
            
            public Controller(Guid eventId, MIDINoteAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }
            
            protected override bool Setup(TEvent e, out MIDINote original)
            {
                IMIDINoteConfiguration<TEvent> configuration = Affordance._configuration.Value;

                original = default;
                
                if (!configuration.Channel.TryExtractMIDIChannel(e, out int channel))
                {
                    return false;
                }
                
                if (!configuration.Pitch.TryExtractMIDIPitch(e, out int pitch))
                {
                    return false;
                }

                if (!configuration.Velocity.Extract(e, out float velocity))
                {
                    return false;
                }

                float pressure = 0;
                
                if (configuration.ApplyPressure)
                {
                    _pressure = configuration.Pressure.MakeSession();
                    if (!_pressure.Setup(e, out pressure))
                    {
                        _pressure = null;
                    }
                }

                original = new MIDINote(channel, pitch, (int)velocity, (int)pressure);

                return true;
            }

            protected override bool Update(TEvent e, MIDINote original, out MIDINote updated)
            {
                updated = original;
                
                if (_pressure != null && _pressure.Update(e, out float pressure))
                {
                    updated = updated.WithPressure((int)pressure);
                    return true;
                }

                return false;
            }

        }

        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (!_configuration)
            {
                this.LogError($"{this} expected configuration");
            }
            return _configuration?.Value != null ? new Controller(id, this) : null;
        }
    }
    
}