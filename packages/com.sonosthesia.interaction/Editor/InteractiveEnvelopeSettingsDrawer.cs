using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Interaction.Editor
{
    [CustomPropertyDrawer(typeof(IInteractiveEnvelopeSettings<>), true)]
    public class InteractiveEnvelopeSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            root.Add(UIElementUtils.Separator());
            
            root.Add(UIElementUtils.SectionLabel(property.name.PropertyNameToLabel()));
            
            root.AddRelativeField(property, "_filter", 
                out SerializedProperty filterProp, out PropertyField filterField);
            
            root.AddRelativeField(property, "_oneEuroFilter", 
                out SerializedProperty _, out PropertyField oneEuroFilterField);
            
            root.AddRelativeField(property, "_interaction", 
                out SerializedProperty typeProp, out PropertyField typeField);
            
            root.AddRelativeField(property, "_valueExtractor", 
                out SerializedProperty _, out PropertyField valueScaleExtractorField);
            
            root.AddRelativeField(property, "_attackExtractor", 
                out SerializedProperty _, out PropertyField timeScaleExtractorField);
            
            root.AddRelativeField(property, "_envelope", 
                out SerializedProperty _, out PropertyField envelopeField);

            root.AddRelativeField(property, "_releaseExtractor", 
                out SerializedProperty _, out PropertyField releaseExtractorField);
            
            root.AddRelativeField(property, "_releaseType", 
                out SerializedProperty _, out PropertyField releaseTypeField);

            root.UpdateVisibility(UpdateVisibility, typeProp, filterProp);
            
            return root;

            void UpdateVisibility()
            {
                EnvelopeInteraction type = (EnvelopeInteraction)typeProp.enumValueIndex;
                EnvelopeFilter filter = (EnvelopeFilter)filterProp.enumValueIndex;
                
                oneEuroFilterField.Show(filter is EnvelopeFilter.OneEuro);
                
                valueScaleExtractorField.Show(true);
                
                timeScaleExtractorField.Show(type is not EnvelopeInteraction.Bypass);
                envelopeField.Show(type is not EnvelopeInteraction.Bypass);
                
                releaseExtractorField.Show(type is EnvelopeInteraction.Hold);
                releaseTypeField.Show(type is EnvelopeInteraction.Hold);
            }
        }
    }
}