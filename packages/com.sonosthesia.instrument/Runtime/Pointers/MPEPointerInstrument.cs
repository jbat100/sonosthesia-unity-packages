using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Channel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    // a silly test instrument 

    internal static class MPENoteExtensions
    {
        public static MPENote ApplyDiff(this MPENote note, float diff, MPEPointerInstrument.DragMapping mapping)
        {
            return mapping switch
            {
                MPEPointerInstrument.DragMapping.Slide => note.ChangeSlide(Mathf.RoundToInt(diff)),
                MPEPointerInstrument.DragMapping.Pressure => note.ChangePressure(Mathf.RoundToInt(diff)),
                MPEPointerInstrument.DragMapping.Bend => note.ChangeBend(diff),
                _ => note
            };
        }
    }
    
    public class MPEPointerInstrument : ChannelDriver<MPENote>, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        internal enum DragMapping
        {
            None,
            Slide,
            Pressure,
            Bend
        }

        [Header("Rest")]
        
        [SerializeField] private int _note;
        
        [SerializeField] private int _velocity;

        [SerializeField] private int _slide;

        [SerializeField] private int _pressure;

        [SerializeField] private float _bend;

        [Header("Drag")] 
        
        [SerializeField] private Vector3 _sensitivity;

        [SerializeField] private DragMapping _x;
        
        [SerializeField] private DragMapping _y;
        
        [SerializeField] private DragMapping _z;

        private readonly struct PointerNote
        {
            public readonly Guid EventId;
            public readonly MPENote InitialNote;

            public PointerNote(Guid eventId, MPENote initialNote)
            {
                EventId = eventId;
                InitialNote = initialNote;
            }

            public override string ToString()
            {
                return $"{nameof(PointerNote)} {nameof(EventId)} {EventId} {nameof(InitialNote)} {InitialNote}";
            }
        }
        
        private readonly Dictionary<int, PointerNote> _pointerNotes = new();

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnPointerDown)} " +
            //          $"press {eventData.pointerPressRaycast.worldPosition} " +
            //          $"current {eventData.pointerCurrentRaycast.worldPosition} " +
            //          $"eventData {eventData}");
            
            if (_pointerNotes.TryGetValue(eventData.pointerId, out PointerNote pointerNote))
            {
                Debug.LogWarning($"{this} unexpected existing note {pointerNote} for pointer {pointerNote.EventId}");
                _pointerNotes.Remove(eventData.pointerId);
                EndEvent(pointerNote.EventId);
            }

            MPENote mpeNote = new MPENote(_note, _velocity, _slide, _pressure, _bend);
            Guid eventId = BeginEvent(mpeNote);
            _pointerNotes[eventData.pointerId] = new PointerNote(eventId, mpeNote);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnPointerUp)} " +
            //          $"press {eventData.pointerPressRaycast.worldPosition} " +
            //          $"current {eventData.pointerCurrentRaycast.worldPosition} " +
            //          $"eventData {eventData}");
            
            if (!_pointerNotes.TryGetValue(eventData.pointerId, out PointerNote pointerNote))
            {
                return;
            }
            
            _pointerNotes.Remove(eventData.pointerId);
            EndEvent(pointerNote.EventId);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnPointerMove)} " +
            //          $"press {eventData.pointerPressRaycast.worldPosition} " +
            //          $"current {eventData.pointerCurrentRaycast.worldPosition} " +
            //          $"eventData {eventData}");
            
            if (!_pointerNotes.TryGetValue(eventData.pointerId, out PointerNote pointerNote))
            {
                return;
            }

            MPENote mpeNote = pointerNote.InitialNote;

            Vector3 drag = eventData.pointerCurrentRaycast.worldPosition - eventData.pointerPressRaycast.worldPosition;
            drag.Scale(_sensitivity);

            mpeNote = mpeNote.ApplyDiff(drag.x, _x);
            mpeNote = mpeNote.ApplyDiff(drag.y, _y);
            mpeNote = mpeNote.ApplyDiff(drag.z, _z);
            
            UpdateEvent(pointerNote.EventId, mpeNote);
        }
    }
}