using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    [CustomPropertyDrawer(typeof(FloatFaderSettings))]
    public class FloatFaderSettingsPropertyDrawer : PropertyDrawer
    {
        protected StyleEnum<DisplayStyle> Show(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty faderTypeProp = property.FindPropertyRelative("_faderType");
            PropertyField faderTypeField = new PropertyField(faderTypeProp);
            root.Add(faderTypeField);

            SerializedProperty valueProp = property.FindPropertyRelative("_value");
            PropertyField valueField = new PropertyField(valueProp);
            root.Add(valueField);
            
            SerializedProperty curveProp = property.FindPropertyRelative("_curve");
            PropertyField curveField = new PropertyField(curveProp);
            root.Add(curveField);

            SerializedProperty remapInputProp = property.FindPropertyRelative("_remapInput");
            PropertyField remapInputField = new PropertyField(remapInputProp, "Input Range");
            root.Add(remapInputField);
            
            
            SerializedProperty remapOutputProp = property.FindPropertyRelative("_remapOutput");
            PropertyField remapOutputField = new PropertyField(remapOutputProp, "Output Range");
            root.Add(remapOutputField);

            SerializedProperty clampProp = property.FindPropertyRelative("_clamp");
            PropertyField clampField = new PropertyField(clampProp);
            root.Add(clampField);
            
            SerializedProperty clampRangeProp = property.FindPropertyRelative("_clampRange");
            PropertyField clampRangeField = new PropertyField(clampRangeProp);
            root.Add(clampRangeField);
            
            // Method to update the visibility of fields based on the enum value
            void UpdateVisibility()
            {
                UnityEngine.Debug.Log($"{this} {nameof(UpdateVisibility)}");
                
                FloatFaderType type = (FloatFaderType)faderTypeProp.enumValueIndex;
                bool clamp = clampProp.boolValue;
                
                valueField.style.display = Show(type is FloatFaderType.Constant);
                curveField.style.display = Show(type is FloatFaderType.Curve);
                remapInputField.style.display = Show(type is FloatFaderType.Remap);
                remapOutputField.style.display = Show(type is FloatFaderType.Remap);
                clampField.style.display = Show(type is not FloatFaderType.Constant);
                clampRangeField.style.display = Show(clamp && type is not FloatFaderType.Constant);
            }

            // Initial visibility update
            UpdateVisibility();

            faderTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            clampField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}