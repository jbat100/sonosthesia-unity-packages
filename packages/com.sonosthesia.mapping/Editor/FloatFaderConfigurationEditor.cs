using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    [CustomEditor(typeof(FloatFaderConfiguration))]
    public class FloatFaderConfigurationEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            FaderConfigurationUtils.SetupFloatSettings(root, serializedObject, "Settings", "_settings");
            return root;
        }
    }
}