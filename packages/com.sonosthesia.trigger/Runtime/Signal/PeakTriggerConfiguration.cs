using Sonosthesia.Extractor;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [CreateAssetMenu(fileName = "PeakTriggerConfiguration", menuName = "Sonosthesia/Trigger/PeakTriggerConfiguration")]
    public class PeakTriggerConfiguration : SignalTriggerConfiguration<Peak, PeakFloatStaticExtractorSettings>
    {
        
    }
}