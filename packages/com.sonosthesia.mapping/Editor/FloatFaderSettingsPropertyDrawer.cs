using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Mapping.Editor
{
    [CustomPropertyDrawer(typeof(FloatFaderSettings))]
    public class FloatFaderSettingsPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            root.AddRelativeField(property, "_faderType", 
                out SerializedProperty faderTypeProp, out PropertyField faderTypeField);
            
            root.AddRelativeField(property, "_constantValue", 
                out SerializedProperty _, out PropertyField constantValueField);
            
            root.AddRelativeField(property, "_processing", 
                out SerializedProperty processingProp, out PropertyField processingField);

            root.AddRelativeField(property, "_curve", 
                out SerializedProperty _, out PropertyField curveField);

            root.AddRelativeField(property, "_remap", 
                out SerializedProperty _, out PropertyField remapField);
            
            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField);

            root.UpdateVisibility(UpdateVisibility, faderTypeProp, processingProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatFaderType type = (FloatFaderType)faderTypeProp.enumValueIndex;
                FloatProcessingType processing = (FloatProcessingType)processingProp.enumValueFlag;
                constantValueField.Show(type is FloatFaderType.Constant);
                processing.Show(curveField, remapField, clampField);
            }
        }
    }
}