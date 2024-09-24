using Sonosthesia.Audio;
using UnityEngine.Timeline;

namespace Sonosthesia.PackAudio
{
    [TrackColor(0.2f, 0.4f, 0.2f)]
    [TrackClipType(typeof(AudioAnalysisPackAsset))]
    [TrackBindingType(typeof(ContinuousAnalysisSignal))]
    public class AudioAnalysisPackTrack : TrackAsset
    {
        
    }
}