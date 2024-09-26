using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [TrackColor(0.2f, 0.2f, 0.4f)]
    [TrackClipType(typeof(LegacyAudioAnalysisAsset))]
    [TrackBindingType(typeof(ContinuousAnalysisSignal))]
    public class LegacyAudioAnalysisTrack : TrackAsset
    {
        
    }
}