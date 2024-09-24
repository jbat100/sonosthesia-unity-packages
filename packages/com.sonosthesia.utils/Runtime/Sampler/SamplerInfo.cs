using System.Collections.Generic;

namespace Sonosthesia.Utils
{
    // cache info for faster repeated access
    internal readonly struct SamplerInfo<T> where T : ITimed
    {
        public readonly T StartValue;
        public readonly T EndValue;
        public readonly float StartTime;
        public readonly float EndTime;
        public readonly int Count;

        public SamplerInfo(IReadOnlyList<T> samples)
        {
            Count = samples.Count;
            StartValue = samples[0];
            EndValue = samples[^1];
            StartTime = StartValue.GetTime();
            EndTime = EndValue.GetTime();
        }
    }
}