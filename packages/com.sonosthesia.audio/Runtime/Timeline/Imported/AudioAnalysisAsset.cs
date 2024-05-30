using System;
using Sonosthesia.Pack;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class AudioAnalysisAsset : PlayableAsset, ITimelineClipAsset
    {
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
        
        // note : samples needs to be a serializable field so that it appears in inspector and is properly 
        // recorded in Library folder
        
        [HideInInspector] public AudioAnalysis[] samples;

        public ClipCaps clipCaps => ClipCaps.None;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Debug.Log($"{this} {nameof(CreatePlayable)}");
            return ScriptPlayable<AudioAnalysisBehaviour>.Create(graph, template);
        }

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