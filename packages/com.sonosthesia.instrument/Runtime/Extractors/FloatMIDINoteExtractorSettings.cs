using System;
using Sonosthesia.Extractor;
using Sonosthesia.MIDI;
using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public enum FloatMIDINoteExtractorType
    {
        Custom,
        Constant,
        Channel,
        Note,
        Velocity,
        Pressure
    }

    internal static class FloatMIDINoteExtractorTypeExtensions
    {
        public static bool Extract(this FloatMIDINoteExtractorType _extractorType, MIDINote e, out float value)
        {
            value = _extractorType switch
            {
                FloatMIDINoteExtractorType.Channel => e.Channel,
                FloatMIDINoteExtractorType.Note => e.Note,
                FloatMIDINoteExtractorType.Velocity => e.Velocity,
                FloatMIDINoteExtractorType.Pressure => e.Pressure,
                _ => throw new ArgumentOutOfRangeException()
            };
            return true;
        }        
    }
    
    [Serializable]
    public class FloatMIDINoteStaticExtractorSettings : StaticExtractorSettings<MIDINote, float, FloatProcessorSettings> 
    {
        [SerializeField] private FloatMIDINoteExtractorType _extractorType = FloatMIDINoteExtractorType.Constant;
        
        protected override bool ExtractRaw(MIDINote e, out float value)
        {
            return _extractorType switch
            {
                FloatMIDINoteExtractorType.Custom => ExtractCustom(e, out value),
                FloatMIDINoteExtractorType.Constant => ExtractConstant(e, out value),
                _ => _extractorType.Extract(e, out value)
            };
        }
    }

    [Serializable]
    public class FloatMIDINoteDynamicExtractorSettings : FloatDynamicExtractorSettings<MIDINote>
    {
        [SerializeField] private FloatMIDINoteExtractorType _extractorType = FloatMIDINoteExtractorType.Constant;
        
        protected override IDynamicExtractorSession<MIDINote, float> MakeRawSession()
        {
            return _extractorType switch
            {
                FloatMIDINoteExtractorType.Custom => CustomSession(),
                FloatMIDINoteExtractorType.Constant => ConstantSession(),
                _ => new SelectorSession(_extractorType)
            };
        }

        private class SelectorSession : StatelessExtractorSession<MIDINote, float>
        {
            private readonly FloatMIDINoteExtractorType _extractorType;

            public SelectorSession(FloatMIDINoteExtractorType extractorType)
            {
                _extractorType = extractorType;
            }
            
            protected override bool Extract(MIDINote e, out float value) => _extractorType.Extract(e, out value);
        }
    }
}