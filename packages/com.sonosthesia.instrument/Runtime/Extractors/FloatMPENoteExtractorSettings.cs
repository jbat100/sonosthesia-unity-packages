using System;
using Sonosthesia.Extractor;
using Sonosthesia.MIDI;
using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public enum FloatMPENoteExtractorType
    {
        Custom,
        Constant,
        Note,
        Velocity,
        Pressure,
        Slide,
        Bend
    }
    
    internal static class FloatMPENoteExtractorTypeExtensions
    {
        public static bool Extract(this FloatMPENoteExtractorType _extractorType, MPENote e, out float value)
        {
            value = _extractorType switch
            {
                FloatMPENoteExtractorType.Note => e.Note,
                FloatMPENoteExtractorType.Velocity => e.Velocity,
                FloatMPENoteExtractorType.Pressure => e.Pressure,
                FloatMPENoteExtractorType.Slide => e.Slide,
                FloatMPENoteExtractorType.Bend => e.Bend,
                _ => throw new ArgumentOutOfRangeException()
            };
            return true;
        }        
    }
    
    [Serializable]
    public class FloatMPENoteStaticExtractorSettings : StaticExtractorSettings<MPENote, float, FloatProcessorSettings> 
    {
        [SerializeField] private FloatMPENoteExtractorType _extractorType = FloatMPENoteExtractorType.Constant;
        
        protected override bool ExtractRaw(MPENote e, out float value)
        {
            return _extractorType switch
            {
                FloatMPENoteExtractorType.Custom => ExtractCustom(e, out value),
                FloatMPENoteExtractorType.Constant => ExtractConstant(e, out value),
                _ => _extractorType.Extract(e, out value)
            };
        }
    }
    
    [Serializable]
    public class FloatMPENoteDynamicExtractorSettings : FloatDynamicExtractorSettings<MPENote>
    {
        [SerializeField] private FloatMPENoteExtractorType _extractorType = FloatMPENoteExtractorType.Constant;
        
        protected override IDynamicExtractorSession<MPENote, float> MakeRawSession()
        {
            return _extractorType switch
            {
                FloatMPENoteExtractorType.Custom => CustomSession(),
                FloatMPENoteExtractorType.Constant => ConstantSession(),
                _ => new SelectorSession(_extractorType)
            };
        }

        private class SelectorSession : StatelessExtractorSession<MPENote, float>
        {
            private readonly FloatMPENoteExtractorType _extractorType;

            public SelectorSession(FloatMPENoteExtractorType extractorType)
            {
                _extractorType = extractorType;
            }
            
            protected override bool Extract(MPENote e, out float value) => _extractorType.Extract(e, out value);
        }
    }
}