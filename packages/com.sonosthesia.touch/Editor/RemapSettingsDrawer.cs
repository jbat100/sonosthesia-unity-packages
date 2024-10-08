using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(RemapSettings))]
    public class RemapSettingsDrawer : PropertyDrawer
    {
        private static Label CreateLabel(string title, int minWidth)
        {
            return new Label(title)
            {
                style =
                {
                    width = minWidth,
                    //alignSelf = Align.Center,
                    paddingLeft = 5,
                    paddingTop = 2
                }
            };
        }

        private static FloatField CreateFloatField(float value)
        {
            return new FloatField
            {
                value = value,
                style =
                {
                    flexGrow = 1f,
                    paddingLeft = 5
                    //alignSelf = Align.Center
                }
            };
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    //alignItems = Align.Center,
                    flexGrow = 1
                }
            };

            SerializedProperty fromMinProp = property.FindPropertyRelative("_fromMin");
            SerializedProperty fromMaxProp = property.FindPropertyRelative("_fromMax");
            SerializedProperty toMinProp = property.FindPropertyRelative("_toMin");
            SerializedProperty toMaxProp = property.FindPropertyRelative("_toMax");
            SerializedProperty clampProp = property.FindPropertyRelative("_clamp");

            Label fromLabel = CreateLabel("From", 40);
            FloatField fromMinField = CreateFloatField(fromMinProp.floatValue);
            fromMinField.BindProperty(fromMinProp);
            FloatField fromMaxField = CreateFloatField(fromMaxProp.floatValue);
            fromMaxField.BindProperty(fromMaxProp);

            
            Label toLabel = CreateLabel("To", 30);
            FloatField toMinField = CreateFloatField(toMinProp.floatValue);
            toMinField.BindProperty(toMinProp);
            FloatField toMaxField = CreateFloatField(toMaxProp.floatValue);
            toMaxField.BindProperty(toMaxProp);

            Label clampLabel = CreateLabel("Clamp", 50);
            Toggle clampToggle = new Toggle()
            {
                value = clampProp.boolValue
            };
            clampToggle.BindProperty(clampProp);

            container.Add(fromLabel);
            container.Add(fromMinField);
            container.Add(fromMaxField);
            container.Add(toLabel);
            container.Add(toMinField);
            container.Add(toMaxField);
            container.Add(clampLabel);
            container.Add(clampToggle);

            return container;
        }
    }
}