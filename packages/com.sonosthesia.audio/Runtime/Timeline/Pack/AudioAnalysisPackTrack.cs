using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [TrackColor(0.2f, 0.4f, 0.2f)]
    [TrackClipType(typeof(AudioAnalysisPackAsset))]
    [TrackBindingType(typeof(AudioAnalysisSignal))]
    public class AudioAnalysisPackTrack : TrackAsset
    {
        
    }
}