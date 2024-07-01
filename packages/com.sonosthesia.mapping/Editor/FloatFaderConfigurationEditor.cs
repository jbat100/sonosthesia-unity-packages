using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    [CustomEditor(typeof(FloatFaderConfiguration))]
    public class FloatFaderConfigurationEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var settingsProperty = serializedObject.FindProperty("_settings");
            var settingsField = new PropertyField(settingsProperty);
            root.Add(settingsField);

            return root;
        }
        
    }
}