using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Instrument.Editor
{
    [CustomPropertyDrawer(typeof(MIDIChannelExtractorSettings<>), true)]
    public class MIDIChannelExtractorSettingsDrawer : MIDIExtractorSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Debug.Log($"{this} MIDIChannelExtractorSettingsDrawer CreatePropertyGUI");
            return base.CreatePropertyGUI(property);
        }
    }
}