using Sonosthesia.Extractor.Editor;
using UnityEditor;

namespace Sonosthesia.Instrument.Editor
{
    [CustomPropertyDrawer(typeof(FloatMIDINoteStaticExtractorSettings))]
    public class FloatMIDINoteStaticExtractorSettingsDrawer : BaseFloatStaticExtractorSettingsDrawer
    {
        protected override int ConstantIndex => (int)FloatMIDINoteExtractorType.Constant;
        protected override int CustomIndex => (int)FloatMIDINoteExtractorType.Custom;
    }

    [CustomPropertyDrawer(typeof(FloatMIDINoteDynamicExtractorSettings))]
    public class FloatMIDINoteDynamicExtractorSettingsDrawer : BaseFloatDynamicExtractorSettingsDrawer
    {
        protected override int ConstantIndex => (int)FloatMIDINoteExtractorType.Constant;
        protected override int CustomIndex => (int)FloatMIDINoteExtractorType.Custom;
    }
    
    [CustomPropertyDrawer(typeof(FloatMPENoteStaticExtractorSettings))]
    public class FloatMPENoteStaticExtractorSettingsDrawer : BaseFloatStaticExtractorSettingsDrawer
    {
        protected override int ConstantIndex => (int)FloatMPENoteExtractorType.Constant;
        protected override int CustomIndex => (int)FloatMPENoteExtractorType.Custom;
    }

    [CustomPropertyDrawer(typeof(FloatMPENoteDynamicExtractorSettings))]
    public class FloatMPENoteDynamicExtractorSettingsDrawer : BaseFloatDynamicExtractorSettingsDrawer
    {
        protected override int ConstantIndex => (int)FloatMPENoteExtractorType.Constant;
        protected override int CustomIndex => (int)FloatMPENoteExtractorType.Custom;
    }
}