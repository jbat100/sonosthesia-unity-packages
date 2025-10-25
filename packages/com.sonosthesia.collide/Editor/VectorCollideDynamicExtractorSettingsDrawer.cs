using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Collide.Editor
{
    [CustomPropertyDrawer(typeof(VectorCollideDynamicExtractorSettings))]
    public class VectorCollideDynamicExtractorSettingsDrawer : PropertyDrawer
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
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp, postProcessingProp);
            
            return root;

            void UpdateVisibility()
            {
                VectorCollideDynamicExtractorSettings.ExtractorType extractorType = 
                    (VectorCollideDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;

                extractorField.Show(extractorType is VectorCollideDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is VectorCollideDynamicExtractorSettings.ExtractorType.Constant);
                velocityTypeField.Show(extractorType is VectorCollideDynamicExtractorSettings.ExtractorType.Velocity);
                directionField.Show(extractorType is VectorCollideDynamicExtractorSettings.ExtractorType.Direction);
                
                spaceField.Show(extractorType is VectorCollideDynamicExtractorSettings.ExtractorType.Direction or 
                    VectorCollideDynamicExtractorSettings.ExtractorType.ContactPoint or 
                    VectorCollideDynamicExtractorSettings.ExtractorType.ContactNormal);
            }
        }
    }
}