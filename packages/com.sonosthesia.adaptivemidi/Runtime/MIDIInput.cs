using System;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIInput : MonoBehaviour
    {
        #region Observables
        
        private readonly Subject<MIDINote> _noteSubject = new ();
        public IObservable<MIDINote> NoteObservable => _noteSubject.AsObservable();
        
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

        public void Broadcast(MIDINote note) => _noteSubject.OnNext(note);
        public void Broadcast(MIDIControl control) => _controlSubject.OnNext(control);
        public void Broadcast(MIDIPolyphonicAftertouch aftertouch) => _polyphonicAftertouchSubject.OnNext(aftertouch);
        public void Broadcast(MIDIClock clock) => _clockSubject.OnNext(clock);
        public void Broadcast(MIDISongPositionPointer pointer) => _songPositionPointerSubject.OnNext(pointer);
        public void Broadcast(MIDISync sync) => _syncSubject.OnNext(sync);

    }
}