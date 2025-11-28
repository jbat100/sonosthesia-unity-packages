using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Audio
{
    public class XAASignalHost : XAAHost
    {
        [SerializeField] private InterfaceReference<ISignal<ContinuousAnalysis>> _continuous;
        public ISignal<ContinuousAnalysis> ContinuousAnalysisSignal => _continuous.Value;

        [SerializeField] private InterfaceReference<ISignal<PeakAnalysis>> _peak;
        public ISignal<PeakAnalysis> PeakAnalysisSignal => _peak.Value;

        protected override void PerformBroadcast(ContinuousAnalysis analysis)
        {
            _continuous.Value?.Broadcast(analysis);
        }

        protected override void PerformBroadcast(PeakAnalysis analysis)
        {
            _peak.Value?.Broadcast(analysis);
        }
    }
}