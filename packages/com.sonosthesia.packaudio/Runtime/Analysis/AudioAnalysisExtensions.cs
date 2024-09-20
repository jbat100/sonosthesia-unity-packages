using Sonosthesia.Audio;
using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{
    public static class PackAudioAnalysisExtensions
    {
        public static AudioAnalysis Unpack(this PackedAudioAnalysis analysis)
        {
            return new AudioAnalysis
            {
                time = analysis.Time,
                rms = analysis.RMS,
                lows = analysis.Lows,
                mids = analysis.Mids,
                highs = analysis.Highs,
                centroid = analysis.Centroid,
                offset = analysis.Offset
            };
        }
    }
}