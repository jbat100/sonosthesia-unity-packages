using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ClampSettings))]
    public class ClampSettingsDrawer : HorizontalPropertyDrawer
    {
        protected override void CreateContent(SerializedProperty property, VisualElement container)
        {
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
            
            container.Add(clampToggle);
            container.Add(minField);
            container.Add(maxField);
        }
    }
}