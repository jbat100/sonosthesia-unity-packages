using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Timeline
{
    class EnablingMixerPlayable : PlayableBehaviour
    {
        EnablingTrack.PostPlaybackState m_PostPlaybackState;
        bool m_BoundBehaviourInitialStateIsEnabled;

        private Behaviour m_BoundBehaviour;
        
        public static ScriptPlayable<EnablingMixerPlayable> Create(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<EnablingMixerPlayable>.Create(graph, inputCount);
        }

        public EnablingTrack.PostPlaybackState postPlaybackState
        {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_BoundBehaviour == null)
                return;

            switch (m_PostPlaybackState)
            {
                case EnablingTrack.PostPlaybackState.Enabled:
                    m_BoundBehaviour.enabled = true;
                    break;
                case EnablingTrack.PostPlaybackState.Disabled:
                    m_BoundBehaviour.enabled = false;
                    break;
                case EnablingTrack.PostPlaybackState.Revert:
                    m_BoundBehaviour.enabled = m_BoundBehaviourInitialStateIsEnabled;
                    break;
                case EnablingTrack.PostPlaybackState.LeaveAsIs:
                default:
                    break;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_BoundBehaviour == null)
            {
                m_BoundBehaviour = playerData as Behaviour;
                m_BoundBehaviourInitialStateIsEnabled = m_BoundBehaviour != null && m_BoundBehaviour.enabled;
            }

            if (m_BoundBehaviour == null)
                return;

            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;
                    break;
                }
            }

            m_BoundBehaviour.enabled = hasInput;
        }
    }
}
