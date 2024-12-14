using System;
using Sonosthesia.Interaction;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class TouchMIDINoteAffordance : ChannelAffordance<MIDINote, TouchEvent>
    {
        [SerializeField] private TouchMIDINoteConfiguration _configuration;

        private class Controller : ChannelAffordanceController<MIDINote, TouchEvent, TouchMIDINoteAffordance>
        {
            private ITouchExtractorSession<float> _pressure;
            
            public Controller(Guid eventId, TouchMIDINoteAffordance affordance) : base(eventId, affordance)
            {
            }
            
            protected override bool Setup(TouchEvent e, out MIDINote original)
            {
                TouchMIDINoteConfiguration configuration = Affordance._configuration;

                original = default;
                
                if (!configuration.Channel.TryExtract(e, out int channel))
                {
                    return false;
                }
                
                if (!configuration.Pitch.TryExtract(e, out int pitch))
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

            protected override bool Update(TouchEvent e, MIDINote original, out MIDINote updated)
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

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
    }
}