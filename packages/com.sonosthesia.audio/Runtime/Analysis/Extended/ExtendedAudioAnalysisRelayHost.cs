using UnityEngine;

namespace Sonosthesia.Audio
{
    public class ExtendedAudioAnalysisRelayHost : ExtendedAudioAnalysisHost
    {
        [SerializeField] private ExtendedAudioAnalysisRelay _relay;

        protected override void PerformBroadcast(ContinuousAnalysis analysis) => _relay.Broadcast(analysis);

        protected override void PerformBroadcast(PeakAnalysis analysis) => _relay.Broadcast(analysis);
    }
}