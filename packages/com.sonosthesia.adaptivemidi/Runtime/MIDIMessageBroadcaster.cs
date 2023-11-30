using System;
using Sonosthesia.AdaptiveMIDI.Messages;
using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIMessageBroadcaster : MonoBehaviour, IMIDIMessageBroadcaster
    {
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

        public virtual void BroadcastNoteOn(MIDINote note) => _noteOnSubject.OnNext(note);
        public virtual void BroadcastNoteOff(MIDINote note) => _noteOffSubject.OnNext(note);
        public virtual void BroadcastControl(MIDIControl control) => _controlSubject.OnNext(control);
        public virtual void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch) => _channelAftertouchSubject.OnNext(aftertouch);
        public virtual void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch) => _polyphonicAftertouchSubject.OnNext(aftertouch);
        public virtual void BroadcastPitchBend(MIDIPitchBend pitchBend) => _pitchBendSubject.OnNext(pitchBend);
        public virtual void BroadcastClock(MIDIClock clock) => _clockSubject.OnNext(clock);
        public virtual void BroadcastPositionPointer(MIDISongPositionPointer pointer) => _songPositionPointerSubject.OnNext(pointer);
        public virtual void BroadcastSync(MIDISync sync) => _syncSubject.OnNext(sync);

        private IDisposable PipeNoteOn(IObservable<MIDINote> observable) => observable.Subscribe(_noteOnSubject);
        private IDisposable PipeNoteOff(IObservable<MIDINote> observable) => observable.Subscribe(_noteOffSubject);
        private IDisposable PipeControl(IObservable<MIDIControl> observable) => observable.Subscribe(_controlSubject);
        private IDisposable PipeChannelAftertouch(IObservable<MIDIChannelAftertouch> observable) => observable.Subscribe(_channelAftertouchSubject);
        private IDisposable PipePolyphonicAftertouch(IObservable<MIDIPolyphonicAftertouch> observable) => observable.Subscribe(_polyphonicAftertouchSubject);
        private IDisposable PipePitchBend(IObservable<MIDIPitchBend> observable) => observable.Subscribe(_pitchBendSubject);
        private IDisposable PipeClock(IObservable<MIDIClock> observable) => observable.Subscribe(_clockSubject);
        private IDisposable PipeSongPositionPointer(IObservable<MIDISongPositionPointer> observable) => observable.Subscribe(_songPositionPointerSubject);
        private IDisposable PipeSync(IObservable<MIDISync> observable) => observable.Subscribe(_syncSubject);

        public IDisposable Pipe(IMIDIMessageBroadcaster broadcaster)
        {
            return new CompositeDisposable
            {
                PipeNoteOn(broadcaster.NoteOnObservable),
                PipeNoteOff(broadcaster.NoteOffObservable),
                PipeControl(broadcaster.ControlObservable),
                PipeChannelAftertouch(broadcaster.ChannelAftertouchObservable),
                PipePolyphonicAftertouch(broadcaster.PolyphonicAftertouchObservable),
                PipePitchBend(broadcaster.PitchBendObservable),
                PipeClock(broadcaster.ClockObservable),
                PipeSongPositionPointer(broadcaster.SongPositionPointerObservable),
                PipeSync(broadcaster.SyncObservable)
            };
        }
    }
}