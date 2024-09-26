using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class XAAAsset : PlayableAsset, ITimelineClipAsset
    {
        private XAABehaviour _template;

        private XAABehaviour template
        {
            get
            {
                _template ??= new XAABehaviour();
                _template.continuous = Continuous;
                _template.peaks = Peaks;
                return _template;
            }
        }
        
        // note : samples needs to be a serializable field and is properly recorded in Library folder
        // note : hide in inspector because the very large number of data points kills editor performance
        // TODO : investigate the size of the resulting asset
        
        [HideInInspector] [SerializeField] private ContinuousAnalysis[] _continuous;
        public ContinuousAnalysis[] Continuous 
        { 
            get => _continuous;
            set => _continuous = value; 
        }
        
        [HideInInspector] [SerializeField] private PeakAnalysis[] _peaks;
        public PeakAnalysis[] Peaks 
        { 
            get => _peaks;
            set => _peaks = value; 
        }

        [SerializeField] private XAAInfo _info;
        public XAAInfo Info
        {
            get => _info;
            set => _info = value;
        }
        
        public ClipCaps clipCaps => ClipCaps.None;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Debug.Log($"{this} {nameof(CreatePlayable)}");
            return ScriptPlayable<XAABehaviour>.Create(graph, template);
        }

        public override double duration
        {
            get
            {
                if (Info != null)
                {
                    return Info.Duration;
                }
                if (Continuous == null || Continuous.Length == 0)
                {
                    return 0;
                }
                return Continuous[^1].time;
            }
        }
    }
}