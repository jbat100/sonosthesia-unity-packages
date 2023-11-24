using System;
using System.Diagnostics;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public interface IMIDIBroadcaster
    {
        void BroadcastNoteOn(MIDINote note);
        void BroadcastNoteOff(MIDINote note);
        void BroadcastControl(MIDIControl control);
        void BroadcastChannelAftertouch(MIDIChannelAftertouch aftertouch);
        void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch);
        void BroadcastPitchBend(MIDIPitchBend pitchBend);
        void BroadcastClock(MIDIClock clock);
        void BroadcastPositionPointer(MIDISongPositionPointer pointer);
        void BroadcastSync(MIDISync sync);
    }
    
    public static class MIDIUtils
    {
        public static int To14BitInt(byte msb, byte lsb, bool signed)
        {
            // Ensure the values are 7-bit
            msb &= 0x7F;
            lsb &= 0x7F;

            // Combine MSB and LSB to get a 14-bit integer [0 16383]
            int combinedValue = (msb << 7) | lsb;

            return signed ? combinedValue - 8192 : combinedValue;
        }

        public static int ClampTo7Bit(int value)
        {
            return Mathf.Clamp(value, 0, 127);
        }
        
        public static float ClampTo7Bit(float value)
        {
            return Mathf.Clamp(value, 0, 127);
        }

        public static TimeSpan TimestampNow => TimeSpan.FromTicks(Stopwatch.GetTimestamp());

        public static void Decode(IMIDIBroadcaster broadcaster, TimeSpan timestamp, byte data0)
        {
            
        }
        
        public static void Decode(IMIDIBroadcaster broadcaster, TimeSpan timestamp, byte data0, byte data1)
        {
            
        }
        
        public static void Decode(IMIDIBroadcaster broadcaster, TimeSpan timestamp, byte data0, byte data1, byte data2)
        {
            
        }
    }
}