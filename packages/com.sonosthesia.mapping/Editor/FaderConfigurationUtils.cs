using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    public static class FaderConfigurationUtils
    {
        public static void SetupFloatSettings(VisualElement root, SerializedObject serializedObject, 
            string title, string path)
        {
            var settingsLabel = new Label(title);
            settingsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            settingsLabel.style.marginTop = 5;
            settingsLabel.style.marginBottom = 5;
            root.Add(settingsLabel);
            
            var settingsProperty = serializedObject.FindProperty(path);
            var settingsField = new PropertyField(settingsProperty);
            root.Add(settingsField);

        }
    }
}