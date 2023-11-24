using System;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIInput : MonoBehaviour
    {
        #region Observables
        
        private readonly Subject<MIDINote> _noteOnSubject = new ();
        public IObservable<MIDINote> NoteOnObservable => _noteOnSubject.AsObservable();
        
        private readonly Subject<MIDINote> _noteOffSubject = new ();
        public IObservable<MIDINote> NoteOffObservable => _noteOffSubject.AsObservable();
        
        private readonly Subject<MIDIControl> _controlSubject = new ();
        public IObservable<MIDIControl> ControlObservable => _controlSubject.AsObservable();
        
        private readonly Subject<MIDIChannelAftertouch> _channelAftertouchSubject = new ();
        public IObservable<MIDIChannelAftertouch> ChannelAftertouchObservable => _channelAftertouchSubject.AsObservable();

        private readonly Subject<MIDIPolyphonicAftertouch> _polyphonicAftertouchSubject = new ();
        public IObservable<MIDIPolyphonicAftertouch> PolyphonicAftertouchObservable => _polyphonicAftertouchSubject.AsObservable();
        
        private readonly Subject<MIDIPitchBend> _pitchBendSubject = new ();
        public IObservable<MIDIPitchBend> PitchBendObservable => _pitchBendSubject.AsObservable();

        private readonly Subject<MIDIClock> _clockSubject = new ();
        public IObservable<MIDIClock> ClockObservable => _clockSubject.AsObservable();
        
        private readonly Subject<MIDISongPositionPointer> _songPositionPointerSubject = new ();
        public IObservable<MIDISongPositionPointer> SongPositionPointerObservable => _songPositionPointerSubject.AsObservable();

        private readonly Subject<MIDISync> _syncSubject = new ();
        public IObservable<MIDISync> SyncObservable => _syncSubject.AsObservable();

        #endregion

        protected void BroadcastNoteOn(MIDINote note) => _noteOnSubject.OnNext(note);
        protected void BroadcastNoteOff(MIDINote note) => _noteOffSubject.OnNext(note);
        protected void BroadcastControl(MIDIControl control) => _controlSubject.OnNext(control);
        protected void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch) => _channelAftertouchSubject.OnNext(aftertouch);
        protected void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch) => _polyphonicAftertouchSubject.OnNext(aftertouch);
        protected void BroadcastPitchBend(MIDIPitchBend pitchBend) => _pitchBendSubject.OnNext(pitchBend);
        protected void BroadcastClock(MIDIClock clock) => _clockSubject.OnNext(clock);
        protected void BroadcastPositionPointer(MIDISongPositionPointer pointer) => _songPositionPointerSubject.OnNext(pointer);
        protected void BroadcastSync(MIDISync sync) => _syncSubject.OnNext(sync);

    }
}