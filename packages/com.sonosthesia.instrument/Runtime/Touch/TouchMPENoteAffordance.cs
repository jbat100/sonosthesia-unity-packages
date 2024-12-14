using System;
using Sonosthesia.Interaction;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class TouchMPENoteAffordance : ChannelAffordance<MPENote, TouchEvent>
    {
        [SerializeField] private TouchMPENoteConfiguration _configuration;

        private class Controller : ChannelAffordanceController<MPENote, TouchEvent, TouchMPENoteAffordance>
        {
            private ITouchExtractorSession<float> _pressure;
            private ITouchExtractorSession<float> _slide;
            private ITouchExtractorSession<float> _bend;
            
            public Controller(Guid eventId, TouchMPENoteAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override bool Setup(TouchEvent e, out MPENote original)
            {
                TouchMPENoteConfiguration configuration = Affordance._configuration;

                original = default;
                
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
                    if (_pressure == null || !_pressure.Setup(e, out pressure))
                    {
                        _pressure = null;
                    }
                }

                float slide = 0;
                if (configuration.ApplySlide)
                {
                    _slide = configuration.Slide.MakeSession();
                    if (_slide == null || !_slide.Setup(e, out slide))
                    {
                        _slide = null;
                    }
                }
                
                float bend = 0;
                if (configuration.ApplyBend)
                {
                    _bend = configuration.Bend.MakeSession();
                    if (_bend == null || !_bend.Setup(e, out bend))
                    {
                        _bend = null;
                    }
                }

                original = new MPENote(pitch, (int)velocity, (int)slide, (int)pressure, bend);

                return true;
            }

            protected override bool Update(TouchEvent e, MPENote original, out MPENote updated)
            {
                updated = original;
                
                if (_pressure != null && _pressure.Update(e, out float pressure))
                {
                    updated = updated.ChangePressure((int)pressure);
                }
                
                if (_slide != null && _slide.Update(e, out float slide))
                {
                    updated = updated.ChangeSlide((int)slide);
                }

                if (_bend != null && _bend.Update(e, out float bend))
                {
                    updated = updated.ChangeBend(bend);
                }

                return true;
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            if (!_configuration)
            {
                this.LogError($"{this} no configuration");
            }
            return _configuration ? new Controller(id, this) : null;
        }
    }
}