using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(VectorTouchExtractorSettings))]
    public class VectorTouchExtractorSettingsDrawer : PropertyDrawer
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
            
            root.AddRelativeField(property, "_velocityType", 
                out SerializedProperty _, out PropertyField velocityTypeField);

            root.AddRelativeField(property, "_space", 
                out SerializedProperty _, out PropertyField spaceField);
            
            root.AddRelativeField(property, "_direction", 
                out SerializedProperty _, out PropertyField directionField);
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);
            
            root.AddRelativeField(property, "_scale",
                out SerializedProperty _, out PropertyField scaleField);
            
            
            void UpdateVisibility()
            {
                VectorTouchExtractorSettings.ExtractorType extractorType = 
                    (VectorTouchExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;

                extractorField.Show(extractorType is VectorTouchExtractorSettings.ExtractorType.Custom);
                velocityTypeField.Show(extractorType is VectorTouchExtractorSettings.ExtractorType.Velocity);
                spaceField.Show(extractorType is VectorTouchExtractorSettings.ExtractorType.Static);
                directionField.Show(extractorType is VectorTouchExtractorSettings.ExtractorType.Static);

                VectorTouchExtractorSettings.PostProcessingType postProcessingType =
                    (VectorTouchExtractorSettings.PostProcessingType)postProcessingProp.enumValueFlag;
                
                scaleField.Show(postProcessingType.HasFlag(VectorTouchExtractorSettings.PostProcessingType.Scale));
            }

            UpdateVisibility();

            extractorTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            postProcessingField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}