using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sonosthesia.Pack;
using Sonosthesia.Audio;

namespace Sonosthesia.PackAudio
{
    [Serializable]
    public class AudioAnalysisPackAsset : PlayableAsset, ITimelineClipAsset
    {
        [HideInInspector] public byte[] data;

        private ContinuousAnalysis[] _samples;
        private ContinuousAnalysis[] samples
        {
            get
            {
                if (_samples == null)
                {
                    _samples = PackedFileUtils.ReadBytes<PackedLegacyAudioAnalysis[]>(data).Select(a => a.Unpack()).ToArray();
                }

                return _samples;
            }
        }
        
        private LegacyAudioAnalysisBehaviour _template;
        private LegacyAudioAnalysisBehaviour template
        {
            get
            {
                _template ??= new LegacyAudioAnalysisBehaviour();
                _template.samples = samples;
                return _template;
            }
        }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Debug.Log($"{this} {nameof(CreatePlayable)}");
            return ScriptPlayable<LegacyAudioAnalysisBehaviour>.Create(graph, template);
        }

        public ClipCaps clipCaps => ClipCaps.None;
        
        public override double duration
        {
            get
            {
                if (samples == null || samples.Length == 0)
                {
                    return 0;
                }

                return samples[^1].time;
            }
        }
    }
}