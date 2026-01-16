using System;
using Sonosthesia.Processing;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Extractor
{
    public enum PeakExtractorType
    {
        Custom,
        Constant,
        Magnitude,
        Strength,
        Duration
    }
    
    [Serializable]
    public class PeakFloatStaticExtractorSettings : StaticExtractorSettings<Peak, float, FloatProcessorSettings>
    { 
        [SerializeField] private PeakExtractorType _extractorType = PeakExtractorType.Constant;

        private bool ExtractProperty(Peak peak, out float value)
        {
            value = _extractorType switch
            {
                PeakExtractorType.Magnitude => peak.Magnitude,
                PeakExtractorType.Strength => peak.Strength,
                PeakExtractorType.Duration => peak.Duration,
                _ => throw new ArgumentOutOfRangeException()
            };
            return true;
        }
        
        protected override bool ExtractRaw(Peak e, out float value)
        {
            return _extractorType switch
            {
                PeakExtractorType.Custom => ExtractCustom(e, out value),
                PeakExtractorType.Constant => ExtractConstant(e, out value),
                PeakExtractorType.Magnitude => ExtractProperty(e, out value),
                PeakExtractorType.Strength => ExtractProperty(e, out value),
                PeakExtractorType.Duration => ExtractProperty(e, out value),
                _ => throw new ArgumentOutOfRangeException(nameof(_extractorType), _extractorType, null)
            };
        }
    }
}