using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Signal
{
    [CreateAssetMenu(fileName = "PeakSignalRelay", menuName = "Sonosthesia/Relays/PeakSignalRelay")]
    public class PeakSignalRelay : StatelessSignalRelay<Peak>
    {
        protected override bool ShouldBroadcast(Peak value)
        {
            // TODO : implement StatelessSignal instead, this is a bit of a hack
            return value.Magnitude > 0f || value.Strength > 0f;
        }
    }
}