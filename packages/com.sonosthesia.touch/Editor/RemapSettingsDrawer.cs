using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(RemapSettings))]
    public class RemapSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = UIHorizontalUtils.CreateContainer(10);

            Label titleLabel = UIHorizontalUtils.CreateLabel(property.name.PropertyNameToLabel(), 50, 5);

            SerializedProperty fromMinProp = property.FindPropertyRelative("_fromMin");
            SerializedProperty fromMaxProp = property.FindPropertyRelative("_fromMax");
            SerializedProperty toMinProp = property.FindPropertyRelative("_toMin");
            SerializedProperty toMaxProp = property.FindPropertyRelative("_toMax");
            SerializedProperty clampProp = property.FindPropertyRelative("_clamp");

            // Label fromLabel = UIHorizontalUtils.CreateLabel("", 10);
            FloatField fromMinField = UIHorizontalUtils.CreateFloatField(fromMinProp);
            FloatField fromMaxField = UIHorizontalUtils.CreateFloatField(fromMaxProp);

            // Label middleLabel = new Label("-")
            // {
            //     style =
            //     {
            //         width = 20,
            //         //justifyContent = Justify.Center,
            //         paddingTop = 2,
            //         unityTextAlign = TextAnchor.MiddleCenter
            //     }
            // };
            
            Label toLabel = UIHorizontalUtils.CreateLabel("To", 20);
            FloatField toMinField = UIHorizontalUtils.CreateFloatField(toMinProp);
            FloatField toMaxField = UIHorizontalUtils.CreateFloatField(toMaxProp);
            toMaxField.BindProperty(toMaxProp);

            Label clampLabel = UIHorizontalUtils.CreateLabel("Clamp", 42);
            Toggle clampToggle = new Toggle()
            {
                value = clampProp.boolValue
            };
            clampToggle.BindProperty(clampProp);

            container.Add(titleLabel);
            //container.Add(fromLabel);
            container.Add(fromMinField);
            container.Add(fromMaxField);
            container.Add(toLabel);
            //container.Add(middleLabel);
            container.Add(toMinField);
            container.Add(toMaxField);
            container.Add(clampLabel);
            container.Add(clampToggle);

            return container;
        }
    }
}