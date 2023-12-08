using System;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public interface IRawMIDIBroadcaster
    {
        void Broadcast(byte data0);
        void Broadcast(byte data0, byte data1);
        void Broadcast(byte data0, byte data1, byte data2);
    }
    
    public interface IRawTimestampedMIDIBroadcaster
    {
        void Broadcast(TimeSpan timestamp, byte data0);
        void Broadcast(TimeSpan timestamp, byte data0, byte data1);
        void Broadcast(TimeSpan timestamp, byte data0, byte data1, byte data2);
    }
    
    public class MIDIEncoder
    {
        private bool Try7Bit(int value, out byte data)
        {
            if (value is < 0 or > 127)
            {
                data = default;
                return false;
            }

            data = (byte) value;
            return true;
        }
        
        private bool Try14Bit(int value, bool signed, out byte lsb, out byte msb)
        {
            if (signed)
            {
                value += 8192;
            }

            if (value is < 0 or > 16383)
            {
                lsb = default;
                msb = default;
                return false;
            }
            
            lsb = (byte) (value & 0x7f);
            msb = (byte) ((value >> 7) & 0x7f);
            return true;
        }

        private bool TryStatusChannel(byte status, int channel, out byte result)
        {
            if (Try7Bit(channel, out byte channelByte))
            {
                result = (byte)((status << 4) | channelByte);
                return true;
            }
            result = default;
            return false;
        }
        
        public void EncodeNoteOn(IRawMIDIBroadcaster broadcaster, MIDINoteOn note)
        {
            if (TryStatusChannel(0x9, note.Channel, out byte data0) && 
                Try7Bit(note.Note, out byte data1) &&
                Try7Bit(note.Velocity, out byte data2))
            {
                broadcaster.Broadcast(data0, data1, data2);
            }
        }

        public void EncodeNoteOff(IRawMIDIBroadcaster broadcaster, MIDINoteOff note)
        {
            if (TryStatusChannel(0x8, note.Channel, out byte data0) && 
                Try7Bit(note.Note, out byte data1) &&
                Try7Bit(note.Velocity, out byte data2))
            {
                broadcaster.Broadcast(data0, data1, data2);
            }
        }

        public void EncodeControl(IRawMIDIBroadcaster broadcaster, MIDIControl control)
        {
            if (TryStatusChannel(0xb, control.Channel, out byte data0) && 
                Try7Bit(control.Number, out byte data1) &&
                Try7Bit(control.Value, out byte data2))
            {
                broadcaster.Broadcast(data0, data1, data2);
            }
        }

        public void EncodeChannelAftertouch(IRawMIDIBroadcaster broadcaster, MIDIChannelAftertouch aftertouch)
        {
            if (TryStatusChannel(0xd, aftertouch.Channel, out byte data0) && 
                Try7Bit(aftertouch.Value, out byte data1))
            {
                broadcaster.Broadcast(data0, data1);
            }
        }

        public void EncodePolyphonicAftertouch(IRawMIDIBroadcaster broadcaster, MIDIPolyphonicAftertouch aftertouch)
        {
            if (TryStatusChannel(0xa, aftertouch.Channel, out byte data0) && 
                Try7Bit(aftertouch.Note, out byte data1) &&
                Try7Bit(aftertouch.Value, out byte data2))
            {
                broadcaster.Broadcast(data0, data1, data2);
            }
        }

        public void EncodePitchBend(IRawMIDIBroadcaster broadcaster, MIDIPitchBend pitchBend)
        {
            if (TryStatusChannel(0xe, pitchBend.Channel, out byte data0) && 
                Try14Bit(pitchBend.Value, true, out byte data1, out byte data2))
            {
                broadcaster.Broadcast(data0, data1, data2);
            }
        }

        public void EncodeClock(IRawMIDIBroadcaster broadcaster, MIDIClock clock)
        {
            broadcaster.Broadcast(0xf8);
        }

        public void EncodePositionPointer(IRawMIDIBroadcaster broadcaster, MIDISongPositionPointer pointer)
        {
            if (Try14Bit(pointer.Position, false, out byte data1, out byte data2))
            {
                broadcaster.Broadcast(0xf2, data1, data2);
            }
        }

        public void EncodeSync(IRawMIDIBroadcaster broadcaster, MIDISync sync)
        {
            switch (sync.Type)
            {
                case MIDISyncType.Start:
                    broadcaster.Broadcast(0xfa);
                    break;
                case MIDISyncType.Stop:
                    broadcaster.Broadcast(0xfc);
                    break;
                case MIDISyncType.Continue:
                    broadcaster.Broadcast(0xfb);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}