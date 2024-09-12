using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [TrackColor(0.4f, 0.2f, 0.2f)]
    [TrackClipType(typeof(AudioAnalysisFloatAsset))]
    [TrackBindingType(typeof(AudioAnalysisSignal))]
    public class AudioAnalysisFloatTrack : TrackAsset
    {
        
    }
}