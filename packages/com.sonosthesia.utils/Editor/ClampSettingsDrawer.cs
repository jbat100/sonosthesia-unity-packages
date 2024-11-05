using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public class ClampSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = UIHorizontalUtils.CreateContainer(10);

            Label titleLabel = UIHorizontalUtils.CreateLabel(property.name.PropertyNameToLabel(), 50, 5);

            SerializedProperty minProp = property.FindPropertyRelative("_min");
            SerializedProperty maxProp = property.FindPropertyRelative("_max");
            SerializedProperty clampProp = property.FindPropertyRelative("_clamp");

            FloatField minField = UIHorizontalUtils.CreateFloatField(minProp);
            FloatField maxField = UIHorizontalUtils.CreateFloatField(maxProp);

            Toggle clampToggle = new Toggle()
            {
                value = clampProp.boolValue
            };
            clampToggle.BindProperty(clampProp);

            container.Add(titleLabel);
            container.Add(clampToggle);
            container.Add(minField);
            container.Add(maxField);

            return container;
        }
    }
}