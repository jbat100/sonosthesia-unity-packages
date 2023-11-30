using System;
using UnityEngine;
using RtMidiDll = RtMidi.Unmanaged;

namespace Sonosthesia.RtMIDI
{
    internal sealed unsafe class RtMIDIOutputPort : IDisposable
    {
        private RtMidiDll.Wrapper* _rtmidi;
        private string _portName;
        
        public RtMIDIOutputPort(int portNumber, string portName)
        {
            _portName = portName;
            _rtmidi = RtMidiDll.OutCreateDefault();

            if (_rtmidi == null || !_rtmidi->ok)
            {
                Debug.LogWarning("Failed to create an RtMidi output device.");
                return;
            }

            RtMidiDll.Api api = RtMidiDll.OutGetCurrentApi(_rtmidi);
            Debug.Log($"Created an RtMidi output device with api {api}.");
            RtMidiDll.OpenPort(_rtmidi, (uint)portNumber, portName);
            Debug.Log($"Opened an RtMidi output port with number {portNumber} name {portName}.");
        }

        ~RtMIDIOutputPort()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;
            RtMidiDll.OutFree(_rtmidi);
        }

        public void Dispose()
        {
            Debug.LogWarning($"{nameof(RtMIDIOutputPort)} {nameof(Dispose)}");

            if (_rtmidi == null || !_rtmidi->ok) return;

            RtMidiDll.OutFree(_rtmidi);
            _rtmidi = null;

            System.GC.SuppressFinalize(this);
        }
        
        public void Broadcast(byte data0)
        {
            byte* message = stackalloc byte [1];
            message[0] = data0;
            Debug.Log($"{nameof(RtMIDIInputPort)} {nameof(Broadcast)} 1 byte : {data0:x2}");
            RtMidiDll.OutSendMessage(_rtmidi, message, 1);
        }

        public void Broadcast(byte data0, byte data1)
        {
            byte* message = stackalloc byte [2];
            message[0] = data0;
            message[1] = data1;
            Debug.Log($"{nameof(RtMIDIInputPort)} {nameof(Broadcast)} 2 bytes : {data0:x2} {data1:x2}");
            RtMidiDll.OutSendMessage(_rtmidi, message, 2);
        }

        public void Broadcast(byte data0, byte data1, byte data2)
        {
            byte* message = stackalloc byte [3];
            message[0] = data0;
            message[1] = data1;
            message[2] = data2;
            Debug.Log($"{nameof(RtMIDIInputPort)} {nameof(Broadcast)} 3 bytes : {data0:x2} {data1:x2} {data2:x2}");
            RtMidiDll.OutSendMessage(_rtmidi, message, 3);
        }
    }
    
}