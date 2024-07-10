using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Sonosthesia.Timeline
{
    /// <summary>
    /// Track that can be used to control the active state of a GameObject.
    /// </summary>
    [Serializable]
    [TrackClipType(typeof(EnablingPlayableAsset))]
    [TrackBindingType(typeof(Behaviour))]
    [ExcludeFromPreset]
    public class EnablingTrack : TrackAsset
    {
        [SerializeField]
        PostPlaybackState m_PostPlaybackState = PostPlaybackState.LeaveAsIs;
        EnablingMixerPlayable m_EnablingMixer;

        /// <summary>
        /// Specify what state to leave the Behaviour in after the Timeline has finished playing.
        /// </summary>
        public enum PostPlaybackState
        {
            /// <summary>
            /// Set the GameObject to active.
            /// </summary>
            Enabled,

            /// <summary>
            /// Set the GameObject to Inactive.
            /// </summary>
            Disabled,

            /// <summary>
            /// Revert the GameObject to the state in was in before the Timeline was playing.
            /// </summary>
            Revert,

            /// <summary>
            /// Leave the GameObject in the state it was when the Timeline was stopped.
            /// </summary>
            LeaveAsIs
        }

        /// <summary>
        /// Specifies what state to leave the GameObject in after the Timeline has finished playing.
        /// </summary>
        public PostPlaybackState postPlaybackState
        {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; UpdateTrackMode(); }
        }

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = EnablingMixerPlayable.Create(graph, inputCount);
            m_EnablingMixer = mixer.GetBehaviour();

            UpdateTrackMode();

            return mixer;
        }

        internal void UpdateTrackMode()
        {
            if (m_EnablingMixer != null)
                m_EnablingMixer.postPlaybackState = m_PostPlaybackState;
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // var gameObject = TrackUtils.GetGameObjectBinding(this, director);
            // if (gameObject != null)
            // {
            //     driver.AddFromName(gameObject, "m_IsActive");
            // }
            
            // TODO : not sure how to do this with a Behaviour as enabled is not backed by a field but by an engine call 
        }

        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Enabled";
            base.OnCreateClip(clip);
        }
    }
}
