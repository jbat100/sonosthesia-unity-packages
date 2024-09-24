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
        
        public float GetTime() => start;

        public Peak ToPeak() => new Peak(duration, magnitude, strength);
    }
}