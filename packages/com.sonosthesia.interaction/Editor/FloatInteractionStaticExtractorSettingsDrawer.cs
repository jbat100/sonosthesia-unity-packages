using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(FloatInteractionStaticExtractorSettings<,>), true)]
    public class FloatInteractionStaticExtractorSettingsDrawer : PropertyDrawer
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

            root.AddRelativeField(property, "_axes", 
                out SerializedProperty _, out PropertyField axesField, false);
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty _, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatInteractionStaticExtractorType extractorType = (FloatInteractionStaticExtractorType)extractorTypeProp.enumValueIndex;
                
                extractorField.Show(extractorType is FloatInteractionStaticExtractorType.Custom);
                constantValueField.Show(extractorType is FloatInteractionStaticExtractorType.Constant);
                velocityTypeField.Show(extractorType is FloatInteractionStaticExtractorType.Velocity);
                axesField.Show(extractorType is FloatInteractionStaticExtractorType.Distance);
            }
        }
    }
}