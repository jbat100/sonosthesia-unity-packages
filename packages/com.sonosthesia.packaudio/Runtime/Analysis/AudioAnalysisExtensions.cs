using Sonosthesia.Audio;
using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{

    public static class LegacyPackAudioAnalysisExtensions
    {
        public static ContinuousAnalysis Unpack(this PackedLegacyAudioAnalysis analysis)
        {
            return new ContinuousAnalysis(analysis.Time, analysis.RMS,
                analysis.Lows, analysis.Mids, analysis.Highs, analysis.Centroid);
        }
    }
    
    public static class PackAudioAnalysisExtensions
    {

        
        public static ContinuousAnalysis Unpack(this PackedContinuousAudioAnalysis analysis)
        {
            return new ContinuousAnalysis(analysis.Time, analysis.RMS,
                analysis.Lows, analysis.Mids, analysis.Highs, analysis.Centroid);
        }

        public static PeakAnalysis Unpack(this PackedAudioPeak peak)
        {
            return new PeakAnalysis(peak.Channel, peak.Start, peak.Duration, peak.Magnitude, peak.Strength);
        }

        public static XAAInfo Unpack(this PackedXAAInfo packed)
        {
            return new XAAInfo(
                packed.Duration,
                packed.Main.Unpack(),
                packed.Lows.Unpack(),
                packed.Mids.Unpack(),
                packed.Highs.Unpack(),
                packed.Centroid.UnpackFrequencyRange());
        }

        private static XAAInfo.SignalInfo Unpack(this PackedAudioSignalInfo packed)
        {
            return new XAAInfo.SignalInfo(
                packed.Band.UnpackFrequencyRange(),
                packed.Magnitude.UnpackMagnitudeRange(),
                packed.Peaks);
        }

        private static XAAInfo.FrequencyRange UnpackFrequencyRange(this PackedRange packed)
        {
            return new XAAInfo.FrequencyRange(packed.Lower, packed.Upper);
        }

        private static XAAInfo.MagnitudeRange UnpackMagnitudeRange(this PackedRange packed)
        {
            return new XAAInfo.MagnitudeRange(packed.Lower, packed.Upper);
        }
        
        
    }
}