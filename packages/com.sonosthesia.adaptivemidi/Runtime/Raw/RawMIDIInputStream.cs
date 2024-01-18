using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public readonly struct RawMIDISingle
    {
        public readonly TimeSpan Timestamp;
        public readonly byte Data0;

        public RawMIDISingle(TimeSpan timestamp, byte data0)
        {
            Timestamp = timestamp;
            Data0 = data0;
        }
    }
    
    public readonly struct RawMIDIDouble
    {
        public readonly TimeSpan Timestamp;
        public readonly byte Data0;
        public readonly byte Data1;
            
        public RawMIDIDouble(TimeSpan timestamp, byte data0, byte data1)
        {
            Timestamp = timestamp;
            Data0 = data0;
            Data1 = data1;
        }
    }
    
    public readonly struct RawMIDITripple
    {
        public readonly TimeSpan Timestamp;
        public readonly byte Data0;
        public readonly byte Data1;
        public readonly byte Data2;
            
        public RawMIDITripple(TimeSpan timestamp, byte data0, byte data1, byte data2)
        {
            Timestamp = timestamp;
            Data0 = data0;
            Data1 = data1;
            Data2 = data2;
        }
    }
    
    public class RawMIDIInputStream : MonoBehaviour, IRawMIDIBroadcaster, IRawTimestampedMIDIBroadcaster
    {
        private readonly Subject<RawMIDISingle> _singleSubject = new();
        public IObservable<RawMIDISingle> SingleObservable => _singleSubject.AsObservable();
        
        private readonly Subject<RawMIDIDouble> _doubleSubject = new();
        public IObservable<RawMIDIDouble> DoubleObservable => _doubleSubject.AsObservable();
        
        private readonly Subject<RawMIDITripple> _trippleSubject = new();
        public IObservable<RawMIDITripple> TrippleObservable => _trippleSubject.AsObservable();

        public void Broadcast(TimeSpan timestamp, byte data0)
        {
            _singleSubject.OnNext(new RawMIDISingle(timestamp, data0));
        }

        public void Broadcast(TimeSpan timestamp, byte data0, byte data1)
        {
            _doubleSubject.OnNext(new RawMIDIDouble(timestamp, data0, data1));
        }

        public void Broadcast(TimeSpan timestamp, byte data0, byte data1, byte data2)
        {
            _trippleSubject.OnNext(new RawMIDITripple(timestamp, data0, data1, data2));
        }

        public void Broadcast(byte data0)
        {
            Broadcast(MIDIUtils.TimestampNow, data0);
        }

        public void Broadcast(byte data0, byte data1)
        {
            Broadcast(MIDIUtils.TimestampNow, data0, data1);
        }

        public void Broadcast(byte data0, byte data1, byte data2)
        {
            Broadcast(MIDIUtils.TimestampNow, data0, data1, data2);
        }
    }
}