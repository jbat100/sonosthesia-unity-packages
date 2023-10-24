using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Channel;

namespace Sonosthesia.MIDI
{
    public class MIDIInputNoteChannelDriver : ChannelDriver<MIDINote>
    {
        [SerializeField] private MIDIInput _input;

        [SerializeField] private bool _endOnZeroVelocity;

        [Header("Filtering")] 
        
        [SerializeField] private bool _filter;
        
        [SerializeField] private int _channel;

        [SerializeField] private int _lowerPitch;

        [SerializeField] private int _upperPitch = 127;
        
        private readonly CompositeDisposable _subscriptions = new ();

        private readonly Dictionary<int, Guid> _notes = new ();

        protected virtual void Awake()
        {
            if (!_input)
            {
                _input = GetComponentInParent<MIDIInput>();
            }
        }
        
        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            if (!_input)
            {
                return;
            }
            _subscriptions.Add(_input.NoteOnObservable.Subscribe(note =>
            {
                if (ShouldFilterNote(note))
                {
                    return;
                }
                if (_notes.TryGetValue(note.Note, out Guid id))
                {
                    if (_endOnZeroVelocity && note.Velocity == 0)
                    {
                        _notes.Remove(note.Note);
                        EndEvent(id, note);
                    }
                    else
                    {
                        UpdateEvent(id, note);
                    }
                }
                else
                {
                    id = BeginEvent(note);
                    _notes[note.Note] = id;   
                }
            }));
            _subscriptions.Add(_input.NoteOffObservable.Subscribe(note =>
            {
                if (ShouldFilterNote(note))
                {
                    return;
                }
                if (_notes.TryGetValue(note.Note, out Guid id))
                {
                    _notes.Remove(note.Note);
                    EndEvent(id, note);
                }
            }));
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            _subscriptions.Clear();
        }
        
        protected virtual bool ShouldFilterNote(MIDINote note)
        {
            if (!_filter)
            {
                return false;
            }
            if (_channel >= 0 && note.Channel != _channel)
            {
                return true;
            }
            if (note.Note < _lowerPitch || note.Note > _upperPitch)
            {
                return true;
            }
            return false;
        }
    }
}