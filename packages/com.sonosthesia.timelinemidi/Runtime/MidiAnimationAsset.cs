using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Timeline.MIDI
{
    // Playable asset class that contains a MIDI animation clip
    [System.Serializable]
    public sealed class MIDIAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        #region Serialized variables

        public MIDIAnimation template = new MIDIAnimation();

        #endregion

        #region PlayableAsset implementation

        public override double duration {
            get { return template.DurationInSecond; }
        }

        #endregion

        #region ITimelineClipAsset implementation

        public ClipCaps clipCaps { get {
            return ClipCaps.Blending |
                   ClipCaps.ClipIn |
                   ClipCaps.Extrapolation |
                   ClipCaps.Looping |
                   ClipCaps.SpeedMultiplier;
        } }

        #endregion

        #region PlayableAsset overrides

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return ScriptPlayable<MIDIAnimation>.Create(graph, template);
        }

        #endregion
    }
}
