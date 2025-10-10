using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Pointer.Editor
{
    [CustomPropertyDrawer(typeof(FloatPointerDynamicExtractorSettings))]
    public class FloatPointerDynamicExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_extractorType", 
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);
            
            root.AddRelativeField(property, "_followStrategy", 
                out SerializedProperty _, out PropertyField followStrategyField);
            
            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField);
            
            root.AddRelativeField(property, "_constantValue", 
                out SerializedProperty _, out PropertyField constantValueField);

            root.AddRelativeField(property, "_space", 
                out SerializedProperty _, out PropertyField spaceField);

            root.AddRelativeField(property, "_axes", 
                out SerializedProperty _, out PropertyField axesField);
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);

            root.AddRelativeField(property, "_curve", 
                out SerializedProperty _, out PropertyField curveField);

            root.AddRelativeField(property, "_remap", 
                out SerializedProperty _, out PropertyField remapField);
            
            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField);
            
            void UpdateVisibility()
            {
                FloatPointerDynamicExtractorSettings.ExtractorType extractorType = (FloatPointerDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                FloatPointerDynamicExtractorSettings.PostProcessingType postProcessingType = (FloatPointerDynamicExtractorSettings.PostProcessingType)postProcessingProp.enumValueFlag;
                
                followStrategyField.Show(extractorType is not FloatPointerDynamicExtractorSettings.ExtractorType.Constant);
                extractorField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Constant);
                
                spaceField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Raycast);
                axesField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Scroll 
                    or FloatPointerDynamicExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerDynamicExtractorSettings.ExtractorType.Screen);
                

                curveField.Show(postProcessingType.HasFlag(FloatPointerDynamicExtractorSettings.PostProcessingType.Curve));
                remapField.Show(postProcessingType.HasFlag(FloatPointerDynamicExtractorSettings.PostProcessingType.Remap));
                clampField.Show(postProcessingType.HasFlag(FloatPointerDynamicExtractorSettings.PostProcessingType.Clamp));
            }

            UpdateVisibility();

            extractorTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            postProcessingField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}