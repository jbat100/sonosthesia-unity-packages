using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(FloatTouchExtractorSettings))]
    public class FloatTouchExtractorSettingsDrawer : PropertyDrawer
    {
        protected StyleEnum<DisplayStyle> Show(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            root.AddRelativeField(property, "_extractorType", 
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);
            
            root.AddRelativeField(property, "_staticValue", 
                out SerializedProperty _, out PropertyField staticValueField);

            root.AddRelativeField(property, "_dynamicType", 
                out SerializedProperty dynamicTypeProp, out PropertyField dynamicTypeField);
            
            root.AddRelativeField(property, "_dynamicsDomain", 
                out SerializedProperty _, out PropertyField dynamicsDomainField);
            
            root.AddRelativeField(property, "_dynamicsOrder", 
                out SerializedProperty _, out PropertyField dynamicsOrderField);
            
            root.AddRelativeField(property, "_distanceType", 
                out SerializedProperty distanceTypeProp, out PropertyField distanceTypeField);

            root.AddRelativeField(property, "_distanceAxes", 
                out SerializedProperty _, out PropertyField distanceAxesField);

            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);

            root.AddRelativeField(property, "_curve", 
                out SerializedProperty _, out PropertyField curveField);

            root.AddRelativeField(property, "_remap", 
                out SerializedProperty _, out PropertyField remapField);
            
            // Method to update the visibility of fields based on the enum value
            void UpdateVisibility()
            {
                UnityEngine.Debug.Log($"{this} {nameof(UpdateVisibility)}");
                
                FloatTouchExtractorSettings.ExtractorType extratorType = (FloatTouchExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                FloatTouchExtractorSettings.PostProcessingType postProcessingType = (FloatTouchExtractorSettings.PostProcessingType)postProcessingProp.enumValueIndex;
                
                staticValueField.style.display = Show(extratorType is FloatTouchExtractorSettings.ExtractorType.Static);
                
                dynamicTypeField.style.display = Show(extratorType is FloatTouchExtractorSettings.ExtractorType.Dynamic);
                dynamicsOrderField.style.display = Show(extratorType is FloatTouchExtractorSettings.ExtractorType.Dynamic);
                dynamicsDomainField.style.display = Show(extratorType is FloatTouchExtractorSettings.ExtractorType.Dynamic);
                
                distanceTypeField.style.display = Show(extratorType is FloatTouchExtractorSettings.ExtractorType.Distance);
                distanceAxesField.style.display = Show(extratorType is FloatTouchExtractorSettings.ExtractorType.Distance);

                remapField.style.display = Show(postProcessingType is FloatTouchExtractorSettings.PostProcessingType.Remap);
                curveField.style.display = Show(postProcessingType is FloatTouchExtractorSettings.PostProcessingType.Curve);
            }

            // Initial visibility update
            UpdateVisibility();

            extractorTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            postProcessingField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}