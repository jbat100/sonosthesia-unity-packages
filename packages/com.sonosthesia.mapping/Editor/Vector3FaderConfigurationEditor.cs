using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    [CustomEditor(typeof(Vector3FaderConfiguration))]
    public class Vector3FaderConfigurationEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            FaderConfigurationUtils.SetupFloatSettings(root, serializedObject, "X Settings", "_xSettings");
            FaderConfigurationUtils.SetupFloatSettings(root, serializedObject, "Y Settings", "_ySettings");
            FaderConfigurationUtils.SetupFloatSettings(root, serializedObject, "Z Settings", "_zSettings");
            return root;
        }
    }
}