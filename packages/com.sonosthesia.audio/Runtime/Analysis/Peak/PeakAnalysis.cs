using System;
using Sonosthesia.Utils;

namespace Sonosthesia.Audio
{
    [Serializable]
    public struct PeakAnalysis : ITimed
    {
        public int channel;
        public float start;
        public float duration;
        public float magnitude;
        public float strength;

        public PeakAnalysis(int channel, float start, float duration, float magnitude, float strength)
        {
            this.channel = channel;
            this.start = start;
            this.duration = duration;
            this.magnitude = magnitude;
            this.strength = strength;
        }

        public override string ToString()
        {
            return $"{nameof(PeakAnalysis)} {nameof(channel)} {channel} " +
                   $"{nameof(start)} {start:F2} {nameof(duration)} {duration:F2} " +
                   $"{nameof(magnitude)} {magnitude:F2} {nameof(strength)} {strength:F2}";
        }

        public float GetTime() => start;

        public Peak ToPeak() => new Peak(duration, magnitude, strength);
    }
}