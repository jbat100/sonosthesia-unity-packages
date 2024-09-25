using System.Collections.Generic;
using Sonosthesia.Utils;
using UnityEngine.Playables;

namespace Sonosthesia.Audio
{
    public class XAABehaviour : PlayableBehaviour
    {
        public ContinuousAnalysis[] continuous;

        public PeakAnalysis[] peaks;
        
        private UnsafeContinuousSampler<ContinuousAnalysis> _continuousSampler;
        private UnsafeDiscreteSampler<PeakAnalysis> _peakSampler;

        private readonly List<PeakAnalysis> _peakBuffer = new ();
        private readonly HashSet<int> _peakChannelHash = new();

        /// <summary>
        ///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour starts.</para>
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnGraphStart(Playable playable)
        {
            // Debug.Log($"{this} {nameof(OnGraphStart)} time {(float)playable.GetTime()}");
            _continuousSampler = new UnsafeContinuousSampler<ContinuousAnalysis>(continuous);
            _peakSampler = new UnsafeDiscreteSampler<PeakAnalysis>(peaks);
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
            
            XAAHost host = playerData as XAAHost;

            if (!host)
            {
                return;
            }
            
            if (_continuousSampler.TryGet(time, out ContinuousAnalysis audioAnalysis))
            {
                host.Broadcast(audioAnalysis);   
            }

            _peakBuffer.Clear();
            _peakChannelHash.Clear();
            _peakSampler.TryGet(time, _peakBuffer);
            // iterate backward to start with most recent, assumes time is in ascending order
            for (int i = _peakBuffer.Count - 1; i >= 0; i--)
            {
                PeakAnalysis peak = _peakBuffer[i];
                // only broadcast if there has not been a peak on this channel on this run
                if (_peakChannelHash.Add(peak.channel))
                {
                    host.Broadcast(peak);
                }
            }
        }
    }
}