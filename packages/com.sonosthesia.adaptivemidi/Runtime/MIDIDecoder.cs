using System;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI
{
    public interface IMIDIMessageBroadcaster
    {
        public IObservable<MIDINote> NoteOnObservable { get; }
        public IObservable<MIDINote> NoteOffObservable { get; }
        public IObservable<MIDIControl> ControlObservable { get; }
        public IObservable<MIDIChannelAftertouch> ChannelAftertouchObservable { get; }
        public IObservable<MIDIPolyphonicAftertouch> PolyphonicAftertouchObservable { get; }
        public IObservable<MIDIPitchBend> PitchBendObservable { get; }
        public IObservable<MIDIClock> ClockObservable { get; }
        public IObservable<MIDISongPositionPointer> SongPositionPointerObservable { get; }
        public IObservable<MIDISync> SyncObservable { get; }
        
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
    
    public class MIDIDecoder
    {
        private int _clockCount = 0;
        
        public void Decode(IMIDIMessageBroadcaster broadcaster, TimeSpan timestamp, byte data0)
        {
            switch (data0)
            {
                case 0xf8:
                    _clockCount++;
                    broadcaster.BroadcastClock(new MIDIClock(timestamp, _clockCount));
                    break;
                case 0xfa:
                    _clockCount = 0;
                    broadcaster.BroadcastSync(new MIDISync(timestamp, MIDISyncType.Start));
                    break;
                case 0xfb:
                    _clockCount = 0;
                    broadcaster.BroadcastSync(new MIDISync(timestamp, MIDISyncType.Continue));
                    break;
                case 0xfc:
                    _clockCount = 0;
                    broadcaster.BroadcastSync(new MIDISync(timestamp, MIDISyncType.Stop));
                    break;
            }
        }
        
        public void Decode(IMIDIMessageBroadcaster broadcaster, TimeSpan timestamp, byte data0, byte data1)
        {
            int status = data0 >> 4;
            int channel = data0 & 0xf;
            
            if (data1 > 0x7f)
            {
                return;
            }
            switch (status)
            {
                case 0xd:
                    broadcaster.BroadcastChannelAftertouch(new MIDIChannelAftertouch(timestamp, channel, data1));
                    break;
            }
        }
        
        public void Decode(IMIDIMessageBroadcaster broadcaster, TimeSpan timestamp, byte data0, byte data1, byte data2)
        {
            // handle system messages separately
            if ((data0 & 0xf0) == 0xf0)
            {
                if (data0 == 0xf2)
                {
                    _clockCount = 0;
                    broadcaster.BroadcastPositionPointer(new MIDISongPositionPointer(timestamp, MIDIUtils.To14BitInt(data2, data1, false)));
                }
            }

            int status = data0 >> 4;
            int channel = data0 & 0xf;

            if (data1 > 0x7f || data2 > 0x7f)
            {
                return;
            }

            switch (status)
            {
                case 0x9:
                    broadcaster.BroadcastNoteOn(new MIDINote(timestamp, channel, data1, data2));
                    break;
                case 0x8:
                    broadcaster.BroadcastNoteOff(new MIDINote(timestamp, channel, data1, data2));
                    break;
                case 0xb:
                    broadcaster.BroadcastControl(new MIDIControl(timestamp, channel, data1, data2));
                    break;
                case 0xa:
                    broadcaster.BroadcastPolyphonicAftertouch(new MIDIPolyphonicAftertouch(timestamp, channel, data1, data2));
                    break;
                case 0xe:
                    broadcaster.BroadcastPitchBend(new MIDIPitchBend(timestamp, channel, MIDIUtils.To14BitInt(data2, data1, true)));
                    break;
            }
        }
    }
}