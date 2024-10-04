using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Envelope.Editor
{
    [CustomPropertyDrawer(typeof(PhasedEnvelopeSettings))]
    public class PhasedEnvelopeSettingsDrawer : PropertyDrawer
    {
        protected StyleEnum<DisplayStyle> Show(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.AddRelativeField(property, "_envelopeType", 
                out SerializedProperty envelopeTypeProp, out PropertyField envelopeTypeField);
            
            root.AddRelativeField(property, "_attack", 
                out SerializedProperty _, out PropertyField attackField);

            root.AddRelativeField(property, "_decay", 
                out SerializedProperty _, out PropertyField decayField);
            
            root.AddRelativeField(property, "_release", 
                out SerializedProperty _, out PropertyField releaseField);
            
            root.AddRelativeField(property, "_hold", 
                out SerializedProperty _, out PropertyField holdField);
            
            root.AddRelativeField(property, "_sustain", 
                out SerializedProperty _, out PropertyField sustainField);

            // Method to update the visibility of fields based on the enum value
            void UpdateVisibility()
            {
                UnityEngine.Debug.Log($"{this} {nameof(UpdateVisibility)}");
                
                PhasedEnvelopeType type = (PhasedEnvelopeType)envelopeTypeProp.enumValueIndex;
                
                attackField.style.display = Show(type is not PhasedEnvelopeType.SR);
                decayField.style.display = Show(type is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS);
                sustainField.style.display = Show(type is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS or PhasedEnvelopeType.SR);
                releaseField.style.display = Show(type is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.AHR or PhasedEnvelopeType.SR);
                holdField.style.display = Show(type is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.AHR);
                sustainField.style.display = Show(type is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS or PhasedEnvelopeType.SR);
            }

            // Initial visibility update
            UpdateVisibility();

            envelopeTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}