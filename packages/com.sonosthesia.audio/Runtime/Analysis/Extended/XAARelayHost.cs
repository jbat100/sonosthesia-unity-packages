using UnityEngine;

namespace Sonosthesia.Audio
{
    public class XAARelayHost : XAAHost
    {
        [SerializeField] private XAARelay _relay;

        protected override void PerformBroadcast(ContinuousAnalysis analysis) => _relay.Broadcast(analysis);

        protected override void PerformBroadcast(PeakAnalysis analysis) => _relay.Broadcast(analysis);
    }
}