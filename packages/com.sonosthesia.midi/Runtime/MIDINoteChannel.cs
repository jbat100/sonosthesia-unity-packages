using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.MIDI
{
    public class MIDINoteChannel : Channel<MIDINote>
    {
        [SerializeField] private MIDIInput _input;

        [Header("Filtering")]
        
        [SerializeField] private int _channel;

        [SerializeField] private int _lowerPitch;

        [SerializeField] private int _upperPitch = 127;
        
        private readonly CompositeDisposable _subscriptions = new ();

        private readonly Dictionary<int, Guid> _notes = new ();

        protected override void Awake()
        {
            base.Awake();
            if (!_input)
            {
                _input = GetComponentInParent<MIDIInput>();
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _subscriptions.Clear();
            if (!_input)
            {
                return;
            }
            _subscriptions.Add(_input.NoteObservable.Subscribe(note =>
            {
                if (ShouldFilterNote(note))
                {
                    return;
                }
                if (_notes.TryGetValue(note.Note, out Guid id))
                {
                    if (note.Velocity == 0)
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
                    id = BeginEnvent(note);
                    _notes[note.Note] = id;   
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