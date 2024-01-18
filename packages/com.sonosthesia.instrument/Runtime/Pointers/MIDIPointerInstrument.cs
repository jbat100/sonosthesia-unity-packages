using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sonosthesia.Channel;
using Sonosthesia.MIDI;

namespace Sonosthesia.Instrument
{
    public class MIDIPointerInstrument : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ChannelDriver<MIDINote> _driver;
        
        [SerializeField] private int _channel;
        
        [SerializeField] private int _note;
        
        [SerializeField] private int _velocity;

        private readonly Dictionary<int, Guid> _pointerEvents = new(); 
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                _driver.EndStream(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }

            _pointerEvents[eventData.pointerId] = _driver.BeginStream(new MIDINote(_channel, _note, _velocity));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                _driver.EndStream(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
        }
    }
}