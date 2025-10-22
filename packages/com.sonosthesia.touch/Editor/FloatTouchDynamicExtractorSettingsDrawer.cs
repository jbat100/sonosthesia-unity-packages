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
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);
            
            root.AddRelativeField(property, "_followStrategy", 
                out SerializedProperty _, out PropertyField followStrategyField);
            
            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField);
            
            root.AddRelativeField(property, "_constantValue", 
                out SerializedProperty _, out PropertyField constantValueField);

            root.AddRelativeField(property, "_velocityType", 
                out SerializedProperty _, out PropertyField velocityTypeField);

            root.AddRelativeField(property, "_axes", 
                out SerializedProperty _, out PropertyField axesField);
            
            root.AddRelativeField(property, "_actorModulationType", 
                out SerializedProperty actorModulationTypeProp, out PropertyField actorModulationTypeField);
            
            root.AddRelativeField(property, "_actorModulation", 
                out SerializedProperty _, out PropertyField actorModulationField);
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);

            root.AddRelativeField(property, "_curve", 
                out SerializedProperty _, out PropertyField curveField);

            root.AddRelativeField(property, "_remap", 
                out SerializedProperty _, out PropertyField remapField);
            
            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp, postProcessingProp, actorModulationTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                FloatTouchDynamicExtractorSettings.ExtractorType extractorType = (FloatTouchDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                FloatProcessingType postProcessingType = (FloatProcessingType)postProcessingProp.enumValueFlag;
                TouchActorModulationType actorModulationType = (TouchActorModulationType)actorModulationTypeProp.enumValueIndex;

                followStrategyField.Show(extractorType is not FloatTouchDynamicExtractorSettings.ExtractorType.Constant);
                extractorField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Constant);
                velocityTypeField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Velocity);
                axesField.Show(extractorType is FloatTouchDynamicExtractorSettings.ExtractorType.Distance);
                
                actorModulationField.Show(actorModulationType is not TouchActorModulationType.None);

                postProcessingType.Show(curveField, remapField, clampField);
            }
        }
    }
}