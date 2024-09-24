using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [TrackColor(0.2f, 0.4f, 0.4f)]
    [TrackClipType(typeof(ExtendedAudioAnalysisAsset))]
    [TrackBindingType(typeof(ExtendedAudioAnalysisHost))]
    public class ExtendedAudioAnalysisTrack : TrackAsset
    {
        
    }
}