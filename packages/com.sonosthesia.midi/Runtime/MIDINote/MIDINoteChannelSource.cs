using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI;
using UniRx;
using UnityEngine;
using Sonosthesia.Channel;
using Sonosthesia.Utils;

namespace Sonosthesia.MIDI
{
    public class MIDINoteChannelSource : MonoBehaviour
    {
        [SerializeField] private MIDIInput _input;
        [SerializeField] private InterfaceReference<IChannel<MIDINote>> _channel;
        [SerializeField] private bool _endOnZeroVelocity;

        [Header("Filtering")] 
        
        [SerializeField] private bool _filter;
        [SerializeField] private int _channelFilter;
        [SerializeField] private int _lowerPitch;
        [SerializeField] private int _upperPitch = 127;
        
        private readonly CompositeDisposable _subscriptions = new ();
        private readonly Dictionary<ChannelNoteKey, Guid> _notes = new ();
        
        private ChannelDriver<MIDINote> _driver;

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
            if (!_input || !_channel)
            {
                return;
            }
            _driver = new ChannelDriver<MIDINote>(_channel.Value);
            _subscriptions.Add(_input.NoteOnObservable.Subscribe(note =>
            {
                if (ShouldFilterNote(note.Channel, note.Note))
                {
                    return;
                }
                ChannelNoteKey key = new ChannelNoteKey(note);
                if (_notes.TryGetValue(key, out Guid id))
                {
                    if (_endOnZeroVelocity && note.Velocity == 0)
                    {
                        _notes.Remove(key);
                        _driver.EndStream(id, new MIDINote(note));
                    }
                    else
                    {
                        _driver.UpdateStream(id, new MIDINote(note));
                    }
                }
                else
                {
                    id = _driver.BeginStream(new MIDINote(note));
                    _notes[key] = id;   
                }
            }));
            _subscriptions.Add(_input.NoteOffObservable.Subscribe(note =>
            {
                if (ShouldFilterNote(note.Channel, note.Note))
                {
                    return;
                }
                if (_notes.Remove(new ChannelNoteKey(note), out Guid id))
                {
                    _driver.EndStream(id, new MIDINote(note));
                }
            }));
            _subscriptions.Add(_input.PolyphonicAftertouchObservable.Subscribe(aftertouch =>
            {
                if (_notes.TryGetValue(new ChannelNoteKey(aftertouch), out Guid evendId))
                {
                    _driver.UpdateStream(evendId, midiNote => midiNote.WithPressure(aftertouch.Value));
                }
            }));
        }
        
        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
            _notes.Clear();
            _driver?.Dispose();
        }
        
        protected virtual bool ShouldFilterNote(int channel, int note)
        {
            if (!_filter)
            {
                return false;
            }
            if (_channelFilter >= 0 && channel != _channelFilter)
            {
                return true;
            }
            if (note < _lowerPitch || note > _upperPitch)
            {
                return true;
            }
            return false;
        }
    }
}