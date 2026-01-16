using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(VectorTouchDynamicExtractorSettings))]
    public class VectorTouchDynamicExtractorSettingsDrawer : PropertyDrawer
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
            
            root.AddRelativeField(property, "_velocityType", 
                out SerializedProperty _, out PropertyField velocityTypeField, false);
            
            root.AddRelativeField(property, "_direction", 
                out SerializedProperty _, out PropertyField directionField, false);
            
            root.AddRelativeField(property, "_space", 
                out SerializedProperty _, out PropertyField spaceField, false);
            
            root.AddRelativeField(property, "_processor", 
                out SerializedProperty _, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                VectorTouchDynamicExtractorSettings.ExtractorType extractorType = 
                    (VectorTouchDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;

                extractorField.Show(extractorType is VectorTouchDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is VectorTouchDynamicExtractorSettings.ExtractorType.Constant);
                velocityTypeField.Show(extractorType is VectorTouchDynamicExtractorSettings.ExtractorType.Velocity);
                directionField.Show(extractorType is VectorTouchDynamicExtractorSettings.ExtractorType.Direction);
                spaceField.Show(extractorType is VectorTouchDynamicExtractorSettings.ExtractorType.Direction);
            }
        }
    }
}