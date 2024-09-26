using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Audio
{
    // note kept as non readonly public fields for easier serialization with timeline API
    
    [Serializable]
    public struct ContinuousAnalysis : ITimed, ILerpable<ContinuousAnalysis>
    {
        public float time;
        public float rms;
        public float lows;
        public float mids;
        public float highs;
        public float centroid;

        public ContinuousAnalysis(float time, float rms, float lows, float mids, float highs, float centroid)
        {
            this.time = time;
            this.rms = rms;
            this.lows = lows;
            this.mids = mids;
            this.highs = highs;
            this.centroid = centroid;
        }

        public float GetTime() => time;

        public ContinuousAnalysis Lerp(ContinuousAnalysis other, float lerp)
        {
            return new ContinuousAnalysis(
                Mathf.Lerp(time, other.time, lerp),
                Mathf.Lerp(rms, other.rms, lerp),
                Mathf.Lerp(lows, other.lows, lerp),
                Mathf.Lerp(mids, other.mids, lerp),
                Mathf.Lerp(highs, other.highs, lerp),
                Mathf.Lerp(centroid, other.centroid, lerp));
        }

        public override string ToString()
        {
            return $"{nameof(ContinuousAnalysis)} {nameof(time)} {time} {nameof(rms)} {rms} bins ({lows}, {mids}, {highs})";
        }
    }
}