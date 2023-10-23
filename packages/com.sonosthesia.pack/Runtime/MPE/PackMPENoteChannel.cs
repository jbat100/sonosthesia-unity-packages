using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMPENoteChannelDriver : ChannelDriver<MPENote>
    {
        [SerializeField] private PackMIDIReceiver _receiver;

        [SerializeField] private string _track;
        
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

            public MPENote Begin(PackedMIDINote note)
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

        private Dictionary<int, ChannelState> _states = new();

        private CompositeDisposable _mpeSubscriptions = new();

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
            
            _mpeSubscriptions.Add(_receiver.NoteOnObservable.Where(note => note.Track == _track).Subscribe(note =>
            {
                ChannelState state = GetState(note.Channel);
                if (state.CurrentNote.HasValue)
                {
                    Debug.LogWarning($"{nameof(PackMPENoteChannelDriver)} unexpected note on {note}");
                    return;
                }
                state.EventId = BeginEvent(state.Begin(note));
            }));
            
            _mpeSubscriptions.Add(_receiver.NoteOffObservable.Where(note => note.Track == _track).Subscribe(note =>
            {
                ChannelState state = GetState(note.Channel);
                if (!state.CurrentNote.HasValue)
                {
                    Debug.LogWarning($"{nameof(PackMPENoteChannelDriver)} expected note {note}");
                    return;
                }
                EndEvent(state.EventId.Value);
                state.End();
            }));
            
            _mpeSubscriptions.Add(_receiver.ChannelAftertouchObservable.Where(aftertouch => aftertouch.Track == _track).Subscribe(aftertouch =>
            {
                ChannelState state = GetState(aftertouch.Channel);
                state.Pressure = aftertouch.Value;
                if (state.OngoingNote)
                {
                    MPENote note = state.Update();
                    UpdateEvent(state.EventId.Value, note);
                }
            }));
            
            _mpeSubscriptions.Add(_receiver.ControlObservable.Where(control => control.Track == _track).Subscribe(control =>
            {
                ChannelState state = GetState(control.Channel);
                if (control.Number != 74)
                {
                    Debug.LogWarning($"{nameof(PackMPENoteChannelDriver)} unexpected control number {control}");
                    return;
                }
                state.Slide = control.Value;
                if (state.OngoingNote)
                {
                    MPENote note = state.Update();
                    UpdateEvent(state.EventId.Value, note);
                }
            }));
            
            _mpeSubscriptions.Add(_receiver.PitchBendObservable.Where(bend => bend.Track == _track).Subscribe(bend =>
            {
                ChannelState state = GetState(bend.Channel);
                state.Bend = bend.Value;
                if (state.OngoingNote)
                {
                    MPENote note = state.Update();
                    UpdateEvent(state.EventId.Value, note);
                }
            }));
        }

        protected override void OnDisable()
        {
            _mpeSubscriptions.Clear();
            _states.Clear();
            base.OnDisable();
        }
    }
}