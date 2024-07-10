#if UNITY_EDITOR
using System.ComponentModel;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Timeline
{
    /// <summary>
    /// Playable Asset class for Enabling Tracks
    /// </summary>
#if UNITY_EDITOR
    [DisplayName("Enabling Clip")]
#endif
    class EnablingPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// Returns a description of the features supported by Enabling clips
        /// </summary>
        public ClipCaps clipCaps { get { return ClipCaps.None; } }

        /// <summary>
        /// Overrides PlayableAsset.CreatePlayable() to inject needed Playables for an Enabling asset
        /// </summary>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return Playable.Create(graph);
        }
    }
}
