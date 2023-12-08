using Marshal = System.Runtime.InteropServices.Marshal;
using RtMidiDll = RtMidi.Unmanaged;

namespace Sonosthesia.RtMIDI
{
    internal sealed unsafe class RtMIDIInputProbe : System.IDisposable
    {
        RtMidiDll.Wrapper* _rtmidi;

        public RtMIDIInputProbe()
        {
            _rtmidi = RtMidiDll.InCreateDefault();

            if (_rtmidi == null || !_rtmidi->ok)
                UnityEngine.Debug.LogWarning("Failed to create an RtMidi input device object.");
        }

        ~RtMIDIInputProbe()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;
            RtMidiDll.InFree(_rtmidi);
        }

        public void Dispose()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;

            RtMidiDll.InFree(_rtmidi);
            _rtmidi = null;

            System.GC.SuppressFinalize(this);
        }

        public int PortCount 
        {
            get 
            {
                if (_rtmidi == null || !_rtmidi->ok) return 0;
                return (int)RtMidiDll.GetPortCount(_rtmidi);
            }
        }

        public string GetPortName(int portNumber)
        {
            if (_rtmidi == null || !_rtmidi->ok) return null;
            return Marshal.PtrToStringAnsi(RtMidiDll.GetPortName(_rtmidi, (uint)portNumber));
        }
    }
}
