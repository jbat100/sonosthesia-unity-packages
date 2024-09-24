using Sonosthesia.Audio;
using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{
    public static class PackAudioAnalysisExtensions
    {
        // legacy
        public static ContinuousAnalysis Unpack(this PackedAudioAnalysis analysis)
        {
            return new ContinuousAnalysis(analysis.Time, analysis.RMS,
                analysis.Lows, analysis.Mids, analysis.Highs, analysis.Centroid);
        }
        
        public static ContinuousAnalysis Unpack(this PackedContinuousAudioAnalysis analysis)
        {
            return new ContinuousAnalysis(analysis.Time, analysis.RMS,
                analysis.Lows, analysis.Mids, analysis.Highs, analysis.Centroid);
        }

        public static PeakAnalysis Unpack(this PackedAudioPeak peak)
        {
            return new PeakAnalysis(peak.Channel, peak.Start, peak.Duration, peak.Magnitude, peak.Strength);
        }

        public static ExtendedAudioAnalysisInfo Unpack(this PackedAudioAnalysisInfo packed)
        {
            return new ExtendedAudioAnalysisInfo(
                packed.Duration,
                packed.Main.Unpack(),
                packed.Lows.Unpack(),
                packed.Mids.Unpack(),
                packed.Highs.Unpack(),
                packed.Centroid.UnpackFrequencyRange());
        }

        private static ExtendedAudioAnalysisInfo.SignalInfo Unpack(this PackedAudioSignalInfo packed)
        {
            return new ExtendedAudioAnalysisInfo.SignalInfo(
                packed.Band.UnpackFrequencyRange(),
                packed.Magnitude.UnpackMagnitudeRange(),
                packed.Peaks);
        }

        private static ExtendedAudioAnalysisInfo.FrequencyRange UnpackFrequencyRange(this PackedRange packed)
        {
            return new ExtendedAudioAnalysisInfo.FrequencyRange(packed.Lower, packed.Upper);
        }

        private static ExtendedAudioAnalysisInfo.MagnitudeRange UnpackMagnitudeRange(this PackedRange packed)
        {
            return new ExtendedAudioAnalysisInfo.MagnitudeRange(packed.Lower, packed.Upper);
        }
        
        
    }
}