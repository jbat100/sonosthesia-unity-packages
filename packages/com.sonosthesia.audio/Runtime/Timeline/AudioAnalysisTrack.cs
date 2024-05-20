using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [TrackColor(0.4f, 0.4f, 0.4f)]
    [TrackClipType(typeof(AudioAnalysisAsset))]
    [TrackBindingType(typeof(AudioAnalysisSignal))]
    public class AudioAnalysisTrack : TrackAsset
    {
        
    }
}