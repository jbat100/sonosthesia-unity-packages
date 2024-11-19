using Sonosthesia.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(FloatTouchExtractorSettings))]
    public class FloatTouchExtractorSettingsDrawer : PropertyDrawer
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
            
            root.AddRelativeField(property, "_staticValue", 
                out SerializedProperty _, out PropertyField staticValueField);

            root.AddRelativeField(property, "_velocityType", 
                out SerializedProperty _, out PropertyField velocityTypeField);

            root.AddRelativeField(property, "_distanceType", 
                out SerializedProperty distanceTypeProp, out PropertyField distanceTypeField);

            root.AddRelativeField(property, "_distanceAxes", 
                out SerializedProperty _, out PropertyField distanceAxesField);
            
            root.AddRelativeField(property, "_normalizedDistance", 
                out SerializedProperty _, out PropertyField normalizedDistanceField);
            
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
            
            void UpdateVisibility()
            {
                FloatTouchExtractorSettings.ExtractorType extractorType = (FloatTouchExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                FloatTouchExtractorSettings.PostProcessingType postProcessingType = (FloatTouchExtractorSettings.PostProcessingType)postProcessingProp.enumValueFlag;
                FloatTouchExtractorSettings.DistanceType distanceType = (FloatTouchExtractorSettings.DistanceType)distanceTypeProp.enumValueIndex;
                TouchActorModulationType actorModulationType = (TouchActorModulationType)actorModulationTypeProp.enumValueIndex;

                extractorField.Show(extractorType is FloatTouchExtractorSettings.ExtractorType.Custom);
                staticValueField.Show(extractorType is FloatTouchExtractorSettings.ExtractorType.Static);
                velocityTypeField.Show(extractorType is FloatTouchExtractorSettings.ExtractorType.Velocity);
                
                distanceTypeField.Show(extractorType is FloatTouchExtractorSettings.ExtractorType.Distance);
                distanceAxesField.Show(extractorType is FloatTouchExtractorSettings.ExtractorType.Distance);
                normalizedDistanceField.Show(extractorType is FloatTouchExtractorSettings.ExtractorType.Distance && 
                                             distanceType is FloatTouchExtractorSettings.DistanceType.ActorToSource);
                
                actorModulationField.Show(actorModulationType is not TouchActorModulationType.None);

                bool curve = postProcessingType.HasFlag(FloatTouchExtractorSettings.PostProcessingType.Curve);
                bool remap = postProcessingType.HasFlag(FloatTouchExtractorSettings.PostProcessingType.Remap);
                bool clamp = postProcessingType.HasFlag(FloatTouchExtractorSettings.PostProcessingType.Clamp);

                curveField.Show(curve);
                remapField.Show(remap);
                clampField.Show(clamp);
            }

            UpdateVisibility();

            extractorTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            postProcessingField.RegisterValueChangeCallback(_ => UpdateVisibility());
            distanceTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            actorModulationTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}