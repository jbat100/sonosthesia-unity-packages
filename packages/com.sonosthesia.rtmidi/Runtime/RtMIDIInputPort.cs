using System;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UniRx;
using UnityEngine;
using RtMidiDll = RtMidi.Unmanaged;

namespace Sonosthesia.RtMIDI
{
    internal sealed unsafe class RtMIDIInputPort : IDisposable
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
        
        private readonly Subject<MIDIChannelAftertouch> _channelAftertouchSubject = new ();
        public IObservable<MIDIChannelAftertouch> ChannelAftertouchObservable => _channelAftertouchSubject.AsObservable();

        private readonly Subject<MIDIPitchBend> _pitchBendSubject = new ();
        public IObservable<MIDIPitchBend> PitchBendObservable => _pitchBendSubject.AsObservable();
        
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

        public RtMIDIInputPort(int portNumber, string portName)
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

        ~RtMIDIInputPort()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;
            RtMidiDll.InFree(_rtmidi);
        }

        public void Dispose()
        {
            Debug.LogWarning($"{nameof(RtMIDIInputPort)} {nameof(Dispose)}");
            
            _clockSubject.Dispose();
            _controlSubject.Dispose();
            _noteOnSubject.Dispose();
            _noteOffSubject.Dispose();
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

            var size = 4ul;
            var message = stackalloc byte [(int)size];
            
            while (true)
            {
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
                else if (size == 2)
                {
                    var status = message[0] >> 4;
                    var channel = message[0] & 0xf;
                    var data1 = message[1];
                    
                    if (data1 > 0x7f) continue; // Invalid data
                    
                    switch (status)
                    {
                        case 0xd:
                            _channelAftertouchSubject.OnNext(new MIDIChannelAftertouch(channel, data1));
                            break;
                    }
                }
                else if (size == 3)
                {
                    var data1 = message[1];
                    var data2 = message[2];
                    
                    // handle system messages separately
                    if ((message[0] & 0xf0) == 0xf0)
                    {
                        if (message[0] == 0xf2)
                        {
                            _clockCount = 0;
                            _songPositionPointerSubject.OnNext(new MIDISongPositionPointer(MIDIUtils.To14BitInt(data2, data1, false)));
                        }
                    }

                    var status = message[0] >> 4;
                    var channel = message[0] & 0xf;

                    if (data1 > 0x7f || data2 > 0x7f) continue; // Invalid data

                    switch (status)
                    {
                        case 0x9:
                            _noteOnSubject.OnNext(new MIDINote(channel, data1, data2));
                            break;
                        case 0x8:
                            _noteOffSubject.OnNext(new MIDINote(channel, data1, data2));
                            break;
                        case 0xb:
                            _controlSubject.OnNext(new MIDIControl(channel, data1, data2));
                            break;
                        case 0xa:
                            _polyphonicAftertouchSubject.OnNext(new MIDIPolyphonicAftertouch(channel, data1, data2));
                            break;
                        case 0xe:
                            _pitchBendSubject.OnNext(new MIDIPitchBend(channel, MIDIUtils.To14BitInt(data2, data1, true)));
                            break;
                    }
                }
            }
        }

        #endregion
    }
}