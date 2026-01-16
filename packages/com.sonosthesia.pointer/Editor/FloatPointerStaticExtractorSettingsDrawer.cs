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
                out SerializedProperty extractorTypeProp, out PropertyField _);

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
            
            root.AddRelativeField(property, "_processor", 
                out SerializedProperty _, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatPointerStaticExtractorSettings.ExtractorType extractorType = (FloatPointerStaticExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                
                extractorField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Constant);
                
                spaceField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Raycast);
                
                axesField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerStaticExtractorSettings.ExtractorType.Screen);
                selectorField.Show(extractorType is FloatPointerStaticExtractorSettings.ExtractorType.Raycast 
                    or FloatPointerStaticExtractorSettings.ExtractorType.Screen);
            }
        }
    }
}