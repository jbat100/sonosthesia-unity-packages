using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(IInteractiveEnvelopeSettings<>), true)]
    public class InteractiveEnvelopeSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            root.Add(UIElementUtils.Separator());
            
            root.Add(UIElementUtils.SectionLabel(property.name.PropertyNameToLabel()));
            
            root.AddRelativeField(property, "_track", 
                out SerializedProperty _, out PropertyField _);
            
            root.AddRelativeField(property, "_filter", 
                out SerializedProperty filterProp, out PropertyField filterField);
            
            root.AddRelativeField(property, "_oneEuroFilter", 
                out SerializedProperty _, out PropertyField oneEuroFilterField);
            
            root.AddRelativeField(property, "_interaction", 
                out SerializedProperty typeProp, out PropertyField typeField);
            
            root.AddRelativeField(property, "_constantExtractor", 
                out SerializedProperty _, out PropertyField constantExtractorField);
            
            root.AddRelativeField(property, "_valueScaleExtractor", 
                out SerializedProperty _, out PropertyField valueScaleExtractorField);
            
            root.AddRelativeField(property, "_timeScaleExtractor", 
                out SerializedProperty _, out PropertyField timeScaleExtractorField);
            
            root.AddRelativeField(property, "_envelope", 
                out SerializedProperty _, out PropertyField envelopeField);

            root.AddRelativeField(property, "_releaseExtractor", 
                out SerializedProperty _, out PropertyField releaseExtractorField);
            
            root.AddRelativeField(property, "_releaseType", 
                out SerializedProperty _, out PropertyField releaseTypeField);
            
            void UpdateVisibility()
            {
                EnvelopeInteraction type = (EnvelopeInteraction)typeProp.enumValueIndex;
                EnvelopeFilter filter = (EnvelopeFilter)filterProp.enumValueIndex;
                
                oneEuroFilterField.Show(filter is EnvelopeFilter.OneEuro);
                
                constantExtractorField.Show(type is EnvelopeInteraction.Constant);
                
                valueScaleExtractorField.Show(type is not EnvelopeInteraction.Constant);
                timeScaleExtractorField.Show(type is not EnvelopeInteraction.Constant);
                envelopeField.Show(type is not EnvelopeInteraction.Constant);
                
                releaseExtractorField.Show(type is EnvelopeInteraction.Contact);
                releaseTypeField.Show(type is EnvelopeInteraction.Contact);
            }

            UpdateVisibility();
            typeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            filterField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}