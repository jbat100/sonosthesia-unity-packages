using System;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.Metronome
{
    public class MIDISyncSignalDriver : MonoBehaviour
    {
        public enum DriverMode
        {
            Clock,
            Update
        }
        
        [SerializeField] private Signal<Sync> _signal;

        [SerializeField] private MIDIInput _input;

        [SerializeField] private DriverMode _driverMode;

        private MIDISongPositionPointer? _pointer;
        private MIDISync? _sync;

        private readonly CircularBuffer<MIDIClock> _clockHistory = new CircularBuffer<MIDIClock>(CLOCK_HISTORY_DEPTH);
        private readonly CompositeDisposable _subscriptions = new ();
        
        // http://midi.teragonaudio.com/tech/midispec/seq.htm

        private const float CLOCKS_PER_BEAT = 6;
        private const int CLOCK_HISTORY_DEPTH = 5;

        private float? EstimateMIDIBeatLength()
        {
            if (_clockHistory.Count < 2)
            {
                return null;
            }

            MIDIClock oldest = _clockHistory.Back();
            MIDIClock newest = _clockHistory.Front();

            int gap = newest.Count - oldest.Count;
            double interval = (newest.Timestamp - oldest.Timestamp).TotalSeconds;

            if (gap < 1 || interval < 1e-4)
            {
                return null;
            }
            
            Debug.Log($"{this} {nameof(EstimateMIDIBeatLength)} with {newest} {oldest}");
            
            return (float)(interval / gap * CLOCKS_PER_BEAT);
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            _subscriptions.Add(_input.SyncObservable.Subscribe(sync =>
            {
                _sync = sync;
                if (sync.Type == MIDISyncType.Stop)
                {
                    Stop();
                }
            }));
            _subscriptions.Add(_input.SongPositionPointerObservable.Subscribe(pointer =>
            {
                _pointer = pointer;
            }));
            _subscriptions.Add(_input.ClockObservable.Subscribe(clock =>
            {
                _clockHistory.PushFront(clock);
                Process();
            }));
        }
        
        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
            Stop();
        }

        protected virtual void Update()
        {
            if (_driverMode == DriverMode.Update)
            {
                Process();
            }
        }

        protected virtual void Process()
        {
            if (!_pointer.HasValue)
            {
                return;
            }

            if (_clockHistory.Count == 0)
            {
                return;
            }

            if (!_sync.HasValue)
            {
                return;
            }

            if (_sync.Value.Type == MIDISyncType.Stop)
            {
                return;
            }

            MIDIClock clock = _clockHistory.Front();
            float? beatLength = EstimateMIDIBeatLength();
            float clockBeats = (clock.Count - 1) / CLOCKS_PER_BEAT;
            float? bpm = 60f / beatLength;

            // in update mode, try to lerp smoothly by best guessing the bpm and moving the play head forward 
            if (beatLength.HasValue && _driverMode == DriverMode.Update)
            {
                float timeSinceClock = (float)(MIDIUtils.TimestampNow - clock.Timestamp).TotalSeconds;
                float beatsSinceClock = Mathf.Min(timeSinceClock / beatLength.Value, 1f / CLOCKS_PER_BEAT);
                _signal.Broadcast(Sync.Play(_pointer.Value.Position + clockBeats + beatsSinceClock, bpm));
            }
            else 
            {
                _signal.Broadcast(Sync.Play(_pointer.Value.Position + clockBeats, bpm));
            }
        }

        protected virtual void Stop()
        {
            // NOTE : keep _pointer
            // http://midi.teragonaudio.com/tech/midispec/seq.htm
            // The slave should keep track of the nearest previous MIDI beat at which it stopped playback
            // (ie, its stopped "Song Position"), in the anticipation that a MIDI Continue might be received next. 
            
            _clockHistory.Clear();
            _signal.Broadcast(Sync.Stop());
        }

    }
}