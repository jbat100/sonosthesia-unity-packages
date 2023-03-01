using System;
using Sonosthesia.AdaptiveMIDI.Messages;
using UniRx;
using UnityEngine;
using RtMidiDll = RtMidi.Unmanaged;

namespace Sonosthesia.RtMIDI
{
    internal sealed unsafe class RtMIDIPort : IDisposable
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
        
        #region Internal objects and methods

        private RtMidiDll.Wrapper* _rtmidi;
        private string _portName;
        
        #endregion

        #region Public methods

        public RtMIDIPort(int portNumber, string portName)
        {
            _portName = portName;

            _rtmidi = RtMidiDll.InCreateDefault();

            if (_rtmidi == null || !_rtmidi->ok)
            {
                UnityEngine.Debug.LogWarning("Failed to create an RtMidi device object.");
                return;
            }

            RtMidiDll.OpenPort(_rtmidi, (uint)portNumber, portName);
        }

        ~RtMIDIPort()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;
            RtMidiDll.InFree(_rtmidi);
        }

        public void Dispose()
        {
            Debug.LogWarning($"{nameof(RtMIDIPort)} {nameof(Dispose)}");
            
            _clockSubject.Dispose();
            _controlSubject.Dispose();
            _noteSubject.Dispose();
            _polyphonicAftertouchSubject.Dispose();
            _songPositionPointerSubject.Dispose();
            _syncSubject.Dispose();

            if (_rtmidi == null || !_rtmidi->ok) return;

            RtMidiDll.InFree(_rtmidi);
            _rtmidi = null;

            System.GC.SuppressFinalize(this);
        }

        private int _clockCount = 0;
        
        public void ProcessMessageQueue()
        {
            if (_rtmidi == null || !_rtmidi->ok)
            {
                //Debug.Log($"{nameof(ProcessMessageQueue)} bailout");
                return;
            }

            while (true)
            {
                var size = 4ul;
                var message = stackalloc byte [(int)size];

                var stamp = RtMidiDll.InGetMessage(_rtmidi, message, ref size);
                
                if (stamp < 0 || size == 0) break;
                
                //Debug.Log($"{nameof(ProcessMessageQueue)} iteration with stamp {stamp} {size} status {message[0]:X}");
                
                if (size == 1)
                {
                    var status = message[0];
                    switch (status)
                    {
                        case 0xf8:
                            _clockCount++;
                            _clockSubject.OnNext(new MIDIClock(_clockCount));
                            break;
                        case 0xfa:
                            _syncSubject.OnNext(new MIDISync(MIDISyncType.Start));
                            break;
                        case 0xfb:
                            _syncSubject.OnNext(new MIDISync(MIDISyncType.Continue));
                            break;
                        case 0xfc:
                            _syncSubject.OnNext(new MIDISync(MIDISyncType.Stop));
                            break;
                    }
                }
                
                if (size == 3)
                {
                    // handle system messages separately
                    if ((message[0] & 0xf0) == 0xf0)
                    {
                        if (message[0] == 0xf2)
                        {
                            _clockCount = 0;
                            _songPositionPointerSubject.OnNext(new MIDISongPositionPointer((message[2] << 7) | message[1]));
                        }
                    }

                    var status = message[0] >> 4;
                    var channel = message[0] & 0xf;
                    var data1 = message[1];
                    var data2 = message[2];

                    if (data1 > 0x7f || data2 > 0x7f) continue; // Invalid data

                    switch (status)
                    {
                        case 0x9:
                            _noteSubject.OnNext(new MIDINote(channel, data1, data2));
                            break;
                        case 0x8:
                            _noteSubject.OnNext(new MIDINote(channel, data1, 0));
                            break;
                        case 0xb:
                            _controlSubject.OnNext(new MIDIControl(channel, data1, data2));
                            break;
                        case 0xa:
                            _polyphonicAftertouchSubject.OnNext(new MIDIPolyphonicAftertouch(channel, data1, data2));
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
