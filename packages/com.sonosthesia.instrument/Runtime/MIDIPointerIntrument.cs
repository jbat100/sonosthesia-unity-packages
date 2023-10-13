using System;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class MIDIPointerIntrument : Channel<MIDINote>, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private int _channel;
        
        [SerializeField] private int _note;
        
        [SerializeField] private int _velocity;

        private Guid? eventId;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventId.HasValue)
            {
                EndEvent(eventId.Value);
            }

            eventId = BeginEvent(new MIDINote(_channel, _note, _velocity));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventId.HasValue)
            {
                EndEvent(eventId.Value);
                eventId = null;
            }
        }
    }
}