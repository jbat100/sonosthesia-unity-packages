using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Audio
{
    public class ExtendedAudioAnalysisSignalHost : ExtendedAudioAnalysisHost
    {
        [SerializeField] private Signal<ContinuousAnalysis> _continuous;
        public Signal<ContinuousAnalysis> ContinuousAnalysisSignal => _continuous;

        [SerializeField] private Signal<PeakAnalysis> _peak;
        public Signal<PeakAnalysis> PeakAnalysisSignal => _peak;

        protected override void PerformBroadcast(ContinuousAnalysis analysis) => _continuous.Broadcast(analysis);

        protected override void PerformBroadcast(PeakAnalysis analysis) => _peak.Broadcast(analysis);
    }
}