using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    [CustomEditor(typeof(Vector2FaderConfiguration))]
    public class Vector2FaderConfigurationEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            FaderConfigurationUtils.SetupFloatSettings(root, serializedObject, "X Settings", "_xSettings");
            FaderConfigurationUtils.SetupFloatSettings(root, serializedObject, "Y Settings", "_ySettings");
            return root;
        }
    }
    
    
}