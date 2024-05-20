using System;
using Sonosthesia.Pack;
using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class AudioAnalysisBehaviour : PlayableBehaviour
    {
        public AudioAnalysis[] samples;

        [SerializeField]
        private Color _color = Color.white;

        /// <summary>
        ///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour starts.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnGraphStart(Playable playable)
        {
            Debug.Log($"{this} {nameof(OnGraphStart)} time {(float)playable.GetTime()}");
            _latestIndex = 0;
        }

        /// <summary>
        ///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour stops.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnGraphStop(Playable playable)
        {
            Debug.Log($"{this} {nameof(OnGraphStop)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called when the Playable that owns the PlayableBehaviour is created.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log($"{this} {nameof(OnPlayableCreate)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called when the Playable that owns the PlayableBehaviour is destroyed.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnPlayableDestroy(Playable playable)
        {
            Debug.Log($"{this} {nameof(OnPlayableDestroy)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called when the Playable play state is changed to Playables.PlayState.Playing.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log($"{this} {nameof(OnBehaviourPlay)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///         <para>This method is invoked when one of the following situations occurs:
        /// &lt;br&gt;&lt;br&gt;
        ///      The effective play state during traversal is changed to Playables.PlayState.Paused. This state is indicated by FrameData.effectivePlayState.&lt;br&gt;&lt;br&gt;
        ///      The PlayableGraph is stopped while the playable play state is Playing. This state is indicated by PlayableGraph.IsPlaying returning true.</para>
        ///       </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log($"{this} {nameof(OnBehaviourPause)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called during the PrepareData phase of the PlayableGraph.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void PrepareData(Playable playable, FrameData info)
        {
            Debug.Log($"{this} {nameof(PrepareData)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called during the PrepareFrame phase of the PlayableGraph.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            Debug.Log($"{this} {nameof(PrepareFrame)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called during the ProcessFrame phase of the PlayableGraph.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        /// <param name="playerData">The user data of the ScriptPlayableOutput that initiated the process pass.</param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            float time = (float)playable.GetTime();
            
            Debug.Log($"{this} {nameof(ProcessFrame)} time {time}");
            
            AudioAnalysisSignal signal = playerData as AudioAnalysisSignal;

            if (!signal)
            {
                return;
            }

            if (TryGetLatestAudioAnalysis(time, out AudioAnalysis audioAnalysis))
            {
                signal.Broadcast(audioAnalysis);   
            }
        }

        private int _latestIndex = 0;

        private bool TryGetLatestAudioAnalysis(float time, out AudioAnalysis audioAnalysis)
        {
            // assumes increasing time order

            if (samples.Length == 0)
            {
                _latestIndex = 0;
                audioAnalysis = default;
                return false;
            }
            
            int lastIndex = samples.Length - 1;

            if (_latestIndex >= lastIndex)
            {
                _latestIndex = 0;
                audioAnalysis = default;
                return false;
            }

            AudioAnalysis candidate = default;
            
            if (_latestIndex > 0 && time < candidate.time)
            {
                _latestIndex = 0;
            }

            for (int i = _latestIndex; i <= lastIndex - 1; i++)
            {
                AudioAnalysis lower = samples[i];
                AudioAnalysis upper = samples[i + 1];
                if (time >= lower.time && time <= upper.time)
                {
                    _latestIndex = i;
                    audioAnalysis = lower;
                    return true;
                }
            }

            _latestIndex = lastIndex;
            audioAnalysis = samples[lastIndex];
            return true;
        }
    }
    
}
