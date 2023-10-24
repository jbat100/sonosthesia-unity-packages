using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Channel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class MIDIPointerInstrument : ChannelDriver<MIDINote>, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private int _channel;
        
        [SerializeField] private int _note;
        
        [SerializeField] private int _velocity;

        private readonly Dictionary<int, Guid> _pointerEvents = new(); 
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }

            _pointerEvents[eventData.pointerId] = BeginEvent(new MIDINote(_channel, _note, _velocity));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
        }
    }
}