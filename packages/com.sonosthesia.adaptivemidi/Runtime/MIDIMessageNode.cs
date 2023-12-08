using System;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public interface IMIDIMessageReceiver
    {
        public IObservable<MIDINoteOn> NoteOnObservable { get; }
        public IObservable<MIDINoteOff> NoteOffObservable { get; }
        public IObservable<MIDIControl> ControlObservable { get; }
        public IObservable<MIDIChannelAftertouch> ChannelAftertouchObservable { get; }
        public IObservable<MIDIPolyphonicAftertouch> PolyphonicAftertouchObservable { get; }
        public IObservable<MIDIPitchBend> PitchBendObservable { get; }
        public IObservable<MIDIClock> ClockObservable { get; }
        public IObservable<MIDISongPositionPointer> SongPositionPointerObservable { get; }
        public IObservable<MIDISync> SyncObservable { get; }
    }
    
    public interface IMIDIMessageBroadcaster
    {
        void Broadcast(MIDINoteOn note);
        void Broadcast(MIDINoteOff note);
        void Broadcast(MIDIControl control);
        void Broadcast(MIDIChannelAftertouch aftertouch);
        void Broadcast(MIDIPolyphonicAftertouch aftertouch);
        void Broadcast(MIDIPitchBend pitchBend);
        void Broadcast(MIDIClock clock);
        void Broadcast(MIDISongPositionPointer pointer);
        void Broadcast(MIDISync sync);
    }

    public class MIDIMessageNode : MonoBehaviour, IMIDIMessageBroadcaster, IMIDIMessageReceiver
    {
        private readonly Subject<MIDINoteOn> _noteOnSubject = new ();
        public IObservable<MIDINoteOn> NoteOnObservable => _noteOnSubject.AsObservable();
        
        private readonly Subject<MIDINoteOff> _noteOffSubject = new ();
        public IObservable<MIDINoteOff> NoteOffObservable => _noteOffSubject.AsObservable();
        
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

        public virtual void Broadcast(MIDINoteOn note) => _noteOnSubject.OnNext(note);
        public virtual void Broadcast(MIDINoteOff note) => _noteOffSubject.OnNext(note);
        public virtual void Broadcast(MIDIControl control) => _controlSubject.OnNext(control);
        public virtual void Broadcast(MIDIChannelAftertouch aftertouch) => _channelAftertouchSubject.OnNext(aftertouch);
        public virtual void Broadcast(MIDIPolyphonicAftertouch aftertouch) => _polyphonicAftertouchSubject.OnNext(aftertouch);
        public virtual void Broadcast(MIDIPitchBend pitchBend) => _pitchBendSubject.OnNext(pitchBend);
        public virtual void Broadcast(MIDIClock clock) => _clockSubject.OnNext(clock);
        public virtual void Broadcast(MIDISongPositionPointer pointer) => _songPositionPointerSubject.OnNext(pointer);
        public virtual void Broadcast(MIDISync sync) => _syncSubject.OnNext(sync);

        private IDisposable PipeNoteOn(IObservable<MIDINoteOn> observable) => observable.Subscribe(_noteOnSubject);
        private IDisposable PipeNoteOff(IObservable<MIDINoteOff> observable) => observable.Subscribe(_noteOffSubject);
        private IDisposable PipeControl(IObservable<MIDIControl> observable) => observable.Subscribe(_controlSubject);
        private IDisposable PipeChannelAftertouch(IObservable<MIDIChannelAftertouch> observable) => observable.Subscribe(_channelAftertouchSubject);
        private IDisposable PipePolyphonicAftertouch(IObservable<MIDIPolyphonicAftertouch> observable) => observable.Subscribe(_polyphonicAftertouchSubject);
        private IDisposable PipePitchBend(IObservable<MIDIPitchBend> observable) => observable.Subscribe(_pitchBendSubject);
        private IDisposable PipeClock(IObservable<MIDIClock> observable) => observable.Subscribe(_clockSubject);
        private IDisposable PipeSongPositionPointer(IObservable<MIDISongPositionPointer> observable) => observable.Subscribe(_songPositionPointerSubject);
        private IDisposable PipeSync(IObservable<MIDISync> observable) => observable.Subscribe(_syncSubject);

        public IDisposable Pipe(IMIDIMessageReceiver receiver)
        {
            return new CompositeDisposable
            {
                PipeNoteOn(receiver.NoteOnObservable),
                PipeNoteOff(receiver.NoteOffObservable),
                PipeControl(receiver.ControlObservable),
                PipeChannelAftertouch(receiver.ChannelAftertouchObservable),
                PipePolyphonicAftertouch(receiver.PolyphonicAftertouchObservable),
                PipePitchBend(receiver.PitchBendObservable),
                PipeClock(receiver.ClockObservable),
                PipeSongPositionPointer(receiver.SongPositionPointerObservable),
                PipeSync(receiver.SyncObservable)
            };
        }
    }
}