using System;
using Sonosthesia.Pack;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    
    
    [Serializable]
    public class AudioAnalysisPackAsset : PlayableAsset, ITimelineClipAsset
    {
        [HideInInspector] public byte[] data;

        private AudioAnalysis[] _samples;
        private AudioAnalysis[] samples
        {
            get
            {
                if (_samples == null)
                {
                    _samples = PackedFileUtils.ReadAnalysisFile(data);
                }

                return _samples;
            }
        }
        
        private AudioAnalysisBehaviour _template;
        private AudioAnalysisBehaviour template
        {
            get
            {
                _template ??= new AudioAnalysisBehaviour();
                _template.samples = samples;
                return _template;
            }
        }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Debug.Log($"{this} {nameof(CreatePlayable)}");
            return ScriptPlayable<AudioAnalysisBehaviour>.Create(graph, template);
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