using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [CreateAssetMenu(fileName = "PeakTriggerConfiguration", menuName = "Sonosthesia/Trigger/PeakTriggerConfiguration")]
    public class PeakTriggerConfiguration : SignalTriggerConfiguration<Peak, PeakTriggerConfiguration.ExtractorSettings>
    {
        public enum PeakSelector
        {
            Unit,
            Magnitude,
            Strength,
            Duration
        }
    
        [Serializable]
        public class ExtractorSettings : ExtractorSettings<Peak, PeakSelector>
        {
            protected override float Select(Peak input, PeakSelector selector) => selector switch
            {
                PeakSelector.Unit => 1,
                PeakSelector.Magnitude => input.Magnitude,
                PeakSelector.Strength => input.Strength,
                PeakSelector.Duration => input.Duration,
                _ => throw new ArgumentOutOfRangeException(nameof(selector), selector, null)
            };
        }
    }
}