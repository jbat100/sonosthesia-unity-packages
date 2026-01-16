using Sonosthesia.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(FloatTouchDynamicExtractorSettings))]
    public class FloatTouchDynamicExtractorSettingsDrawer : PropertyDrawer
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

            root.AddRelativeField(property, "_velocityType", 
                out SerializedProperty _, out PropertyField velocityTypeField, false);

            root.AddRelativeField(property, "_axes", 
                out SerializedProperty _, out PropertyField axesField, false);
            
            root.AddRelativeField(property, "_actorModulationType", 
                out SerializedProperty actorModulationTypeProp, out PropertyField _);
            
            root.AddRelativeField(property, "_actorModulation", 
                out SerializedProperty _, out PropertyField actorModulationField, false);
            
            root.AddRelativeField(property, "_processor", 
                out SerializedProperty _, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp, actorModulationTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatTouchDynamicExtractorSettings.ExtractorType extractorType = (FloatTouchDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                TouchActorModulationType actorModulationType = (TouchActorModulationType)actorModulationTypeProp.enumValueIndex;

                followStrategyField.Show(extractorType is not FloatTouchDynamicExtractorSettings.ExtractorType.Constant);
                extractorField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Constant);
                velocityTypeField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Velocity);
                axesField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Distance);
                actorModulationField.Show(actorModulationType is not TouchActorModulationType.None);
            }
        }
    }
}