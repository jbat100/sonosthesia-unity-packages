using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sonosthesia.Pack;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class AudioAnalysisFloatAsset : PlayableAsset, ITimelineClipAsset
    {
        [HideInInspector] public float[] data;
        
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
        
        private AudioAnalysis[] _samples;
        private AudioAnalysis[] samples
        {
            get
            {
                if (_samples == null || _samples.Length == 0)
                {
                    _samples = Read();
                }

                return _samples;
            }
        }

        private AudioAnalysis[] Read()
        {
            const int pointSize = 7;
            int count = data.Length / pointSize;
            
            AudioAnalysis[] result = new AudioAnalysis[count];
            
            for (int i = 0; i < count; i++)
            {
                int index = i * pointSize;
                AudioAnalysis audioAnalysis = new AudioAnalysis
                {
                    time = data[index],
                    rms = data[index+1],
                    lows = data[index+2],
                    mids = data[index+3],
                    highs = data[index+4],
                    centroid = data[index+5],
                    offset = data[index+6] != 0f,
                };
                result[i] = audioAnalysis;
            }

            return result;
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