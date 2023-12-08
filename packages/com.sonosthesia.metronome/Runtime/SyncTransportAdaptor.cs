using System;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Metronome
{
    // In the case of MIDI clock sync, there is no available transport info. 
    // This attempts to recreate it by assuming constant time signature

    [Serializable]
    public class TimeSignature
    {
        [SerializeField] private int _numerator = 4;
        [SerializeField] private int _denominator = 4;

        public int SixteenthsPerBeat => _denominator switch
        {
            16 => 1,
            8 => 2,
            4 => 4,
            2 => 8,
            1 => 16,
            _ => 4
        };

        public int BeatsPerBar => _numerator;
    }
    
    public class SyncTransportAdaptor : MapAdaptor<Sync, Transport>
    {
        [SerializeField] private TimeSignature _timeSignature;
        
        protected override Transport Map(Sync source)
        {
            int totalSixteenths = Mathf.FloorToInt(source.Position);
            int sixteenthsPerBeat = _timeSignature.SixteenthsPerBeat;

            int totalBeats = Mathf.FloorToInt((float) totalSixteenths / (float) sixteenthsPerBeat);
            int sixteenths = totalSixteenths - (totalBeats * sixteenthsPerBeat);
            
            int bars = Mathf.FloorToInt((float) totalBeats / (float) _timeSignature.BeatsPerBar);
            int beats = totalBeats - (bars * _timeSignature.BeatsPerBar);

            return new Transport(bars, beats, sixteenths);
        }
    }
}