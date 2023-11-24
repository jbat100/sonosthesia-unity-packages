using System;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMIDIRawInput : MIDIInput
    {
        [SerializeField] private PackMIDIRawReceiver _receiver;

        private readonly CompositeDisposable _subscriptions = new();

        private int _clockCount;
        
        protected virtual void OnEnable()
        {
            _clockCount = 0;
            _subscriptions.Clear();
            _subscriptions.Add(_receiver.SingleObservable.Subscribe(Process));
            _subscriptions.Add(_receiver.DoubleObservable.Subscribe(Process));
            _subscriptions.Add(_receiver.TrippleObservable.Subscribe(Process));
        }
        
        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
        }

        private void Process(PackedMIDIRawSourceSingle data)
        {
            TimeSpan timestamp = TimeSpan.FromSeconds(data.CumulativeTime);
            byte status = data.B0;
            switch (status)
            {
                case 0xf8:
                    _clockCount++;
                    BroadcastClock(new MIDIClock(timestamp, _clockCount));
                    break;
                case 0xfa:
                    _clockCount = 0;
                    BroadcastSync(new MIDISync(timestamp, MIDISyncType.Start));
                    break;
                case 0xfb:
                    _clockCount = 0;
                    BroadcastSync(new MIDISync(timestamp, MIDISyncType.Continue));
                    break;
                case 0xfc:
                    _clockCount = 0;
                    BroadcastSync(new MIDISync(timestamp, MIDISyncType.Stop));
                    break;
            }
        }
        
        private void Process(PackedMIDIRawSourceDouble data)
        {
            TimeSpan timestamp = TimeSpan.FromSeconds(data.CumulativeTime);
            int status = data.B0 >> 4;
            int channel = data.B0 & 0xf;
            byte data1 = data.B1;
            if (data1 > 0x7f)
            {
                return;
            }
            switch (status)
            {
                case 0xd:
                    BroadcastChannelAftertouch(new MIDIChannelAftertouch(timestamp, channel, data1));
                    break;
            }
        }
        
        private void Process(PackedMIDIRawSourceTripple data)
        {
            TimeSpan timestamp = TimeSpan.FromSeconds(data.CumulativeTime);
            byte data0 = data.B0;
            byte data1 = data.B1;
            byte data2 = data.B2;
                    
            // handle system messages separately
            if ((data0 & 0xf0) == 0xf0)
            {
                if (data0 == 0xf2)
                {
                    _clockCount = 0;
                    BroadcastPositionPointer(new MIDISongPositionPointer(timestamp, MIDIUtils.To14BitInt(data2, data1, false)));
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
                    BroadcastNoteOn(new MIDINote(timestamp, channel, data1, data2));
                    break;
                case 0x8:
                    BroadcastNoteOff(new MIDINote(timestamp, channel, data1, data2));
                    break;
                case 0xb:
                    BroadcastControl(new MIDIControl(timestamp, channel, data1, data2));
                    break;
                case 0xa:
                    BroadcastPolyphonicAftertouch(new MIDIPolyphonicAftertouch(timestamp, channel, data1, data2));
                    break;
                case 0xe:
                    BroadcastPitchBend(new MIDIPitchBend(timestamp, channel, MIDIUtils.To14BitInt(data2, data1, true)));
                    break;
            }
        }
    }
}