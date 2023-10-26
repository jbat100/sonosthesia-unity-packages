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
        
        [SerializeField] private int _channelFilter;

        [SerializeField] private int _lowerPitch;

        [SerializeField] private int _upperPitch = 127;
        
        private readonly CompositeDisposable _subscriptions = new ();

        private readonly Dictionary<Key, Guid> _notes = new ();
        
        private struct Key
        {
            public int Channel;
            public int Note;

            public Key(MIDINote note)
            {
                Channel = note.Channel;
                Note = note.Note;
            }
            
            public Key(MIDIPolyphonicAftertouch aftertouch)
            {
                Channel = aftertouch.Channel;
                Note = aftertouch.Note;
            }
        }

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
                Key key = new Key(note);
                if (_notes.TryGetValue(key, out Guid id))
                {
                    if (_endOnZeroVelocity && note.Velocity == 0)
                    {
                        _notes.Remove(key);
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
                    _notes[key] = id;   
                }
            }));
            _subscriptions.Add(_input.NoteOffObservable.Subscribe(note =>
            {
                if (ShouldFilterNote(note))
                {
                    return;
                }
                Key key = new Key(note);
                if (_notes.TryGetValue(key, out Guid id))
                {
                    _notes.Remove(key);
                    EndEvent(id, note);
                }
            }));
            _subscriptions.Add(_input.PolyphonicAftertouchObservable.Subscribe(aftertouch =>
            {
                if (_notes.TryGetValue(new Key(aftertouch), out Guid evendId))
                {
                    UpdateEvent(evendId, midiNote => midiNote.WithPressure(aftertouch.Value));
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
            if (_channelFilter >= 0 && note.Channel != _channelFilter)
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