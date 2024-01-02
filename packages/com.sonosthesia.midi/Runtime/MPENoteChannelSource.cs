using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MPENoteChannelSource : MonoBehaviour
    {
        [SerializeField] private MIDIInput _input;

        [SerializeField] private ChannelDriver<MPENote> _driver;
        
        // typically these parameters will be sent on the channel by the DAW before note so need to cache it
        private class ChannelState
        {
            // 48 semitones up/down mapped to pitch bend [-8192 8191]
            const float BEND_SEMITONES = 96f / 16383f;
            
            public int Slide { get; set; }
            public int Pressure { get; set; }
            public int Bend { get; set; }
            
            public MPENote? CurrentNote { get; set; }
            public Guid? EventId { get; set; }

            public bool OngoingNote => CurrentNote.HasValue;

            public MPENote Begin(MIDINote note)
            {
                MPENote mpeNote = new MPENote(note.Note, note.Velocity, Slide, Pressure, Bend * BEND_SEMITONES);
                CurrentNote = mpeNote;
                return mpeNote;
            }

            public MPENote Update()
            {
                MPENote current = CurrentNote.Value;
                MPENote updated = new MPENote(current.Note, current.Velocity, Slide, Pressure, Bend * BEND_SEMITONES);
                CurrentNote = updated;
                return updated;
            }

            public void End()
            {
                CurrentNote = null;
                EventId = null;
            }
        }

        private readonly Dictionary<int, ChannelState> _states = new();

        private readonly CompositeDisposable _mpeSubscriptions = new();

        private ChannelState GetState(int channel)
        {
            if (_states.TryGetValue(channel, out ChannelState state))
            {
                return state;
            }
            state = new ChannelState();
            _states[channel] = state;
            return state;
        }
        
        protected virtual void OnEnable()
        {
            _mpeSubscriptions.Clear();
            _states.Clear();
            
            _mpeSubscriptions.Add(_input.NoteOnObservable.Subscribe(note =>
            {
                ChannelState state = GetState(note.Channel);
                if (state.CurrentNote.HasValue)
                {
                    Debug.LogWarning($"{nameof(MPENoteChannelSource)} unexpected note on {note}");
                    return;
                }
                state.EventId = _driver.BeginEvent(state.Begin(new MIDINote(note)));
            }));
            
            _mpeSubscriptions.Add(_input.NoteOffObservable.Subscribe(note =>
            {
                ChannelState state = GetState(note.Channel);
                if (!state.CurrentNote.HasValue)
                {
                    Debug.LogWarning($"{nameof(MPENoteChannelSource)} expected note {note}");
                    return;
                }
                _driver.EndEvent(state.EventId.Value);
                state.End();
            }));
            
            _mpeSubscriptions.Add(_input.ChannelAftertouchObservable.Subscribe(aftertouch =>
            {
                ChannelState state = GetState(aftertouch.Channel);
                state.Pressure = aftertouch.Value;
                if (state.OngoingNote)
                {
                    MPENote note = state.Update();
                    _driver.UpdateEvent(state.EventId.Value, note);
                }
            }));
            
            _mpeSubscriptions.Add(_input.ControlObservable.Subscribe(control =>
            {
                ChannelState state = GetState(control.Channel);
                if (control.Number != 74)
                {
                    Debug.LogWarning($"{nameof(MPENoteChannelSource)} unexpected control number {control}");
                    return;
                }
                state.Slide = control.Value;
                if (state.OngoingNote)
                {
                    MPENote note = state.Update();
                    _driver.UpdateEvent(state.EventId.Value, note);
                }
            }));
            
            _mpeSubscriptions.Add(_input.PitchBendObservable.Subscribe(bend =>
            {
                ChannelState state = GetState(bend.Channel);
                state.Bend = bend.Value;
                if (state.OngoingNote)
                {
                    MPENote note = state.Update();
                    _driver.UpdateEvent(state.EventId.Value, note);
                }
            }));
        }

        protected virtual void OnDisable()
        {
            _mpeSubscriptions.Clear();
            _states.Clear();
        }
    }
}