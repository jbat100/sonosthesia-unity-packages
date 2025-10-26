using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Instrument.Editor
{
    [CustomPropertyDrawer(typeof(MIDIPitchExtractorSettings<>), true)]
    public class MIDIPitchExtractorSettingsDrawer : MIDIExtractorSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Debug.Log($"{this} MIDIPitchExtractorSettingsDrawer CreatePropertyGUI");
            return base.CreatePropertyGUI(property);
        }
    }
}