using System;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    // note : trying to determine if we need to have specific assets for each version of 
    // audio pipeline format. Possible issues :
    // - An AudioAnalysisAsset serialized using a previous version of the types may not serialize correctly
    // currently favoring keeping one asset type for simplicity and reacting to actual needs, for now
    // - Offload all the the pipeline file format version handling complexity to the importer

    [Serializable]
    public class LegacyAudioAnalysisAsset : PlayableAsset, ITimelineClipAsset
    {
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
        
        // note : samples needs to be a serializable field and is properly recorded in Library folder
        // note : hide in inspector because the very large number of data points kills editor performance
        // TODO : investigate the size of the resulting asset
        
        [HideInInspector] public ContinuousAnalysis[] samples;

        public ClipCaps clipCaps => ClipCaps.None;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Debug.Log($"{this} {nameof(CreatePlayable)}");
            return ScriptPlayable<LegacyAudioAnalysisBehaviour>.Create(graph, template);
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