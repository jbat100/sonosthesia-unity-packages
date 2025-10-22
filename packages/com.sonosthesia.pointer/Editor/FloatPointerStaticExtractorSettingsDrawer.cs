using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Pointer.Editor
{
    [CustomPropertyDrawer(typeof(FloatPointerStaticExtractorSettings))]
    public class FloatPointerStaticExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_extractorType", 
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);

            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField);
            
            root.AddRelativeField(property, "_constantValue", 
                out SerializedProperty _, out PropertyField constantValueField);

            root.AddRelativeField(property, "_space", 
                out SerializedProperty _, out PropertyField spaceField);

            root.AddRelativeField(property, "_axes", 
                out SerializedProperty _, out PropertyField axesField);
            
            root.AddRelativeField(property, "_selector", 
                out SerializedProperty _, out PropertyField selectorField);
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);

            root.AddRelativeField(property, "_curve", 
                out SerializedProperty _, out PropertyField curveField);

            root.AddRelativeField(property, "_remap", 
                out SerializedProperty _, out PropertyField remapField);
            
            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp, postProcessingProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatPointerStaticExtractorSettings.ExtractorType extractorType = (FloatPointerStaticExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                FloatProcessingType postProcessingType = (FloatProcessingType)postProcessingProp.enumValueFlag;
                
                extractorField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Constant);
                
                spaceField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Raycast);
                
                axesField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerStaticExtractorSettings.ExtractorType.Screen);
                selectorField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerStaticExtractorSettings.ExtractorType.Screen);
                
                postProcessingType.Show(curveField, remapField, clampField);
            }
        }
    }
}