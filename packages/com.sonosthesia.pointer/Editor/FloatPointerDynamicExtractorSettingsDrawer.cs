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
                out SerializedProperty extractorTypeProp, out PropertyField _);
            
            root.AddRelativeField(property, "_followStrategy", 
                out SerializedProperty _, out PropertyField followStrategyField, false);
            
            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField, false);
            
            root.AddRelativeField(property, "_constantValue", 
                out SerializedProperty _, out PropertyField constantValueField, false);

            root.AddRelativeField(property, "_space", 
                out SerializedProperty _, out PropertyField spaceField, false);

            root.AddRelativeField(property, "_axes", 
                out SerializedProperty _, out PropertyField axesField, false);
            
            root.AddRelativeField(property, "_selector", 
                out SerializedProperty _, out PropertyField selectorField, false);

            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty _, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatPointerDynamicExtractorSettings.ExtractorType extractorType = (FloatPointerDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                
                followStrategyField.Show(extractorType is not FloatPointerDynamicExtractorSettings.ExtractorType.Constant);
                extractorField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Constant);
                
                spaceField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Raycast);
                
                axesField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Scroll 
                    or FloatPointerDynamicExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerDynamicExtractorSettings.ExtractorType.Screen);
                selectorField.Show(extractorType is FloatPointerDynamicExtractorSettings.ExtractorType.Scroll
                    or FloatPointerDynamicExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerDynamicExtractorSettings.ExtractorType.Screen);
            }
        }
    }
}