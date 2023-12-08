using System;
using Sonosthesia.AdaptiveMIDI;
using UnityEngine;
using RtMidiDll = RtMidi.Unmanaged;

namespace Sonosthesia.RtMIDI
{
    internal sealed unsafe class RtMIDIInputPort : IDisposable
    {
        private RtMidiDll.Wrapper* _rtmidi;
        private string _portName;
        
        public RtMIDIInputPort(int portNumber, string portName)
        {
            _portName = portName;
            _rtmidi = RtMidiDll.InCreateDefault();

            if (_rtmidi == null || !_rtmidi->ok)
            {
                Debug.LogWarning("Failed to create an RtMidi input device.");
                return;
            }
            
            RtMidiDll.Api api = RtMidiDll.InGetCurrentApi(_rtmidi);
            Debug.Log($"Created an RtMidi output device with api {api}.");
            RtMidiDll.InIgnoreTypes(_rtmidi, false, false, false);
            RtMidiDll.OpenPort(_rtmidi, (uint)portNumber, portName);
            Debug.Log($"Opened an RtMidi input port with number {portNumber} name {portName}.");
        }

        ~RtMIDIInputPort()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;
            RtMidiDll.InFree(_rtmidi);
        }

        public void Dispose()
        {
            Debug.LogWarning($"{nameof(RtMIDIInputPort)} {nameof(Dispose)}");

            if (_rtmidi == null || !_rtmidi->ok) return;

            RtMidiDll.InFree(_rtmidi);
            _rtmidi = null;

            System.GC.SuppressFinalize(this);
        }

        private double _cummulativeTimestamp;
        
        // NOTE (jb.keijiro.rtmidi): The RtMidi callback API is intentionally omitted because it doesn't
        // work well under the unmanaged-to-managed invocation. It'll be broken when
        // the callback is called from an unattached MIDI driver thread.
        
        public void ProcessMessageQueue(IRawTimestampedMIDIBroadcaster broadcaster)
        {
            if (_rtmidi == null || !_rtmidi->ok)
            {
                //Debug.Log($"{nameof(ProcessMessageQueue)} bailout");
                return;
            }

            ulong size = 4ul;
            Byte* message = stackalloc byte [(int)size];
            
            while (true)
            {
                double stamp = RtMidiDll.InGetMessage(_rtmidi, message, ref size);
                
                if (size == 0)
                { 
                    break;
                }
                
                if (stamp < 0)
                {
                    //Debug.LogWarning($"{nameof(ProcessMessageQueue)} iteration with stamp {stamp} {size} status {message[0]:X}");
                    break;
                }
                
                _cummulativeTimestamp += stamp;
                
                TimeSpan timestamp = TimeSpan.FromSeconds(_cummulativeTimestamp);
                
                //Debug.Log($"{nameof(ProcessMessageQueue)} iteration with stamp {stamp} {size} status {message[0]:X}");

                switch (size)
                {
                    case 1:
                        broadcaster.Broadcast(timestamp, message[0]);
                        break;
                    case 2:
                        broadcaster.Broadcast(timestamp, message[0], message[1]);
                        break;
                    case 3:
                        broadcaster.Broadcast(timestamp, message[0], message[1], message[2]);
                        break;
                }
            }
        }
    }
}
