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
        
        private readonly Subject<MIDIPolyphonicAftertouch> _polyphonicAftertouchSubject = new ();
        public IObservable<MIDIPolyphonicAftertouch> PolyphonicAftertouchObservable => _polyphonicAftertouchSubject.AsObservable();
        
        private readonly Subject<MIDIClock> _clockSubject = new ();
        public IObservable<MIDIClock> ClockObservable => _clockSubject.AsObservable();
        
        private readonly Subject<MIDISongPositionPointer> _songPositionPointerSubject = new ();
        public IObservable<MIDISongPositionPointer> SongPositionPointerObservable => _songPositionPointerSubject.AsObservable();

        private readonly Subject<MIDISync> _syncSubject = new ();
        public IObservable<MIDISync> SyncObservable => _syncSubject.AsObservable();

        #endregion

        public void BroadcastNoteOn(MIDINote note) => _noteOnSubject.OnNext(note);
        public void BroadcastNoteOff(MIDINote note) => _noteOffSubject.OnNext(note);
        public void BroadcastControl(MIDIControl control) => _controlSubject.OnNext(control);
        public void BroadcastAftertouch(MIDIPolyphonicAftertouch aftertouch) => _polyphonicAftertouchSubject.OnNext(aftertouch);
        public void BroadcastClock(MIDIClock clock) => _clockSubject.OnNext(clock);
        public void BroadcastPositionPointer(MIDISongPositionPointer pointer) => _songPositionPointerSubject.OnNext(pointer);
        public void BroadcastSync(MIDISync sync) => _syncSubject.OnNext(sync);

    }
}