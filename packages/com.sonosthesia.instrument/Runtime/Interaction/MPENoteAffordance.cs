using System;
using Sonosthesia.Interaction;
using Sonosthesia.MIDI;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
 public class MPENoteAffordance<TEvent> : ChannelAffordance<MPENote, TEvent> where TEvent : struct
    {
        [SerializeField] private InterfaceReference<IMPENoteConfiguration<TEvent>> _configuration;

        private class Controller : ChannelAffordanceController<MPENote, TEvent, MPENoteAffordance<TEvent>>
        {
            private IDynamicExtractorSession<TEvent, float> _pressure;
            private IDynamicExtractorSession<TEvent, float> _slide;
            private IDynamicExtractorSession<TEvent, float> _bend;
            
            public Controller(Guid eventId, MPENoteAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }

            protected override bool Setup(TEvent e, out MPENote original)
            {
                IMPENoteConfiguration<TEvent> configuration = Affordance._configuration.Value;

                original = default;
                
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

            protected override bool Update(TEvent e, MPENote original, out MPENote updated)
            {
                updated = original;
                
                if (_pressure != null && _pressure.Update(e, out float pressure))
                {
                    updated = updated.WithPressure((int)pressure);
                }
                
                if (_slide != null && _slide.Update(e, out float slide))
                {
                    updated = updated.WithSlide((int)slide);
                }

                if (_bend != null && _bend.Update(e, out float bend))
                {
                    updated = updated.WithBend(bend);
                }

                return true;
            }
        }

        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (_configuration == null)
            {
                this.LogError($"{this} no configuration");
            }
            return _configuration?.Value != null ? new Controller(id, this) : null;
        }
    }
}