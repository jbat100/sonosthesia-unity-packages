using System;

namespace Sonosthesia.Pack
{
    // note kept as non readonly public fields for easier serialization with timeline API
    
    [Serializable]
    public struct AudioAnalysis
    {
        public float time;
        public float rms;
        public float lows;
        public float mids;
        public float highs;
        public float centroid;
        public bool offset;

        public override string ToString()
        {
            return $"{nameof(AudioAnalysis)} {nameof(time)} {time} {nameof(rms)} {rms}";
        }
    }
}