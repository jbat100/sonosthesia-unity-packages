using Sonosthesia.Utils;
using UnityEngine.Playables;

namespace Sonosthesia.Audio
{
    public class LegacyAudioAnalysisBehaviour : PlayableBehaviour
    {
        public ContinuousAnalysis[] samples;

        private UnsafeContinuousSampler<ContinuousAnalysis> _sampler;

        /// <summary>
        ///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour starts.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnGraphStart(Playable playable)
        {
            // Debug.Log($"{this} {nameof(OnGraphStart)} time {(float)playable.GetTime()}");
            _sampler = new UnsafeContinuousSampler<ContinuousAnalysis>(samples);
        }

        /// <summary>
        ///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour stops.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnGraphStop(Playable playable)
        {
            // Debug.Log($"{this} {nameof(OnGraphStop)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called when the Playable that owns the PlayableBehaviour is created.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnPlayableCreate(Playable playable)
        {
            // Debug.Log($"{this} {nameof(OnPlayableCreate)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called when the Playable that owns the PlayableBehaviour is destroyed.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnPlayableDestroy(Playable playable)
        {
            // Debug.Log($"{this} {nameof(OnPlayableDestroy)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called when the Playable play state is changed to Playables.PlayState.Playing.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Debug.Log($"{this} {nameof(OnBehaviourPlay)} time {(float)playable.GetTime()}");
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
            // Debug.Log($"{this} {nameof(OnBehaviourPause)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called during the PrepareData phase of the PlayableGraph.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void PrepareData(Playable playable, FrameData info)
        {
            // Debug.Log($"{this} {nameof(PrepareData)} time {(float)playable.GetTime()}");
        }

        /// <summary>
        ///   <para>This function is called during the PrepareFrame phase of the PlayableGraph.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // Debug.Log($"{this} {nameof(PrepareFrame)} time {(float)playable.GetTime()}");
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
            // Debug.Log($"{this} {nameof(ProcessFrame)} time {time}");
            ContinuousAnalysisSignal signal = playerData as ContinuousAnalysisSignal;
            if (!signal)
            {
                return;
            }
            if (_sampler.TryGet(time, out ContinuousAnalysis audioAnalysis))
            {
                signal.Broadcast(audioAnalysis);   
            }
        }
    }
}
