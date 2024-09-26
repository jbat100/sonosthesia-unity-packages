using System;
using UnityEngine;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class XAAInfo
    {
        [Serializable]
        public class Range
        {
            [SerializeField] private float _lower;
            public float Lower => _lower;
            
            [SerializeField] private float _upper;
            public float Upper => _upper;

            public Range(float lower, float upper)
            {
                _lower = lower;
                _upper = upper;
            }
        }
        
        [Serializable]
        public class FrequencyRange : Range
        {
            public override string ToString() => $"({Lower:F1} : {Upper:F1}) Hz";

            public FrequencyRange(float lower, float upper) : base(lower, upper)
            {
            }
        }
        
        [Serializable]
        public class MagnitudeRange : Range
        {
            public override string ToString() => $"({Lower:F1} : {Upper:F1}) dB";

            public MagnitudeRange(float lower, float upper) : base(lower, upper)
            {
            }
        }

        [Serializable]
        public class SignalInfo
        {
            [SerializeField] private FrequencyRange _band;
            public FrequencyRange Band => _band;
            
            [SerializeField] private MagnitudeRange _magnitude;
            public MagnitudeRange Magnitude => _magnitude;
            
            [SerializeField] private int _peaks;
            public int Peaks => _peaks;

            public SignalInfo(FrequencyRange band, MagnitudeRange magnitude, int peaks)
            {
                _band = band;
                _magnitude = magnitude;
                _peaks = peaks;
            }
            
            public override string ToString() => $"Band : {_band}\n Magnitude : {_magnitude}\n Peaks : {_peaks}";
        }

        [SerializeField] private float _duration;
        public float Duration => _duration;

        [SerializeField] private SignalInfo _main;
        public SignalInfo Main => _main;
        
        [SerializeField] private SignalInfo _lows;
        public SignalInfo Lows => _lows;
        
        [SerializeField] private SignalInfo _mids;
        public SignalInfo Mids => _mids;
        
        [SerializeField] private SignalInfo _highs;
        public SignalInfo Highs => _highs;

        [SerializeField] private FrequencyRange _centroid;
        public FrequencyRange Centroid => _centroid;

        public XAAInfo(float duration,
            SignalInfo main, SignalInfo lows, SignalInfo mids, SignalInfo highs,
            FrequencyRange centroid)
        {
            _duration = duration;
            _main = main;
            _lows = lows;
            _mids = mids;
            _highs = highs;
            _centroid = centroid;
        }
    }
}