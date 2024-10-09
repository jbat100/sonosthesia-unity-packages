using Sonosthesia.Utils;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Envelope.Editor
{
    [CustomPropertyDrawer(typeof(EnvelopeSettings))]
    public class EnvelopeSettingsDrawer : PropertyDrawer
    {
        protected StyleEnum<DisplayStyle> Show(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_envelopeType", 
                out SerializedProperty envelopeTypeProp, out PropertyField envelopeTypeField);
            
            root.AddRelativeField(property, "_envelopeFactory", 
                out SerializedProperty _, out PropertyField envelopeFactoryField);
            
            root.AddRelativeField(property, "_animationCurve", 
                out SerializedProperty _, out PropertyField animationCurveField);
            
            root.AddRelativeField(property, "_phasedType", 
                out SerializedProperty phasedTypeProp, out PropertyField phasedTypeField);
            
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
                
                EnvelopeType type = (EnvelopeType)envelopeTypeProp.enumValueIndex;
                
                phasedTypeField.style.display = Show(type == EnvelopeType.Phased);
                animationCurveField.style.display = Show(type == EnvelopeType.Curve);
                envelopeFactoryField.style.display = Show(type == EnvelopeType.Custom);
                
                if (type == EnvelopeType.Phased)
                {
                    PhasedEnvelopeType phasedType = (PhasedEnvelopeType)phasedTypeProp.enumValueIndex;
                    attackField.style.display = Show(phasedType is not PhasedEnvelopeType.SR);
                    decayField.style.display = Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS);
                    sustainField.style.display = Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS or PhasedEnvelopeType.SR);
                    releaseField.style.display = Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.AHR or PhasedEnvelopeType.SR);
                    holdField.style.display = Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.AHR);
                    sustainField.style.display = Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS or PhasedEnvelopeType.SR);
                }
                else
                {
                    attackField.style.display = Show(false);
                    decayField.style.display = Show(false);
                    sustainField.style.display = Show(false);
                    releaseField.style.display = Show(false);
                    holdField.style.display = Show(false);
                    sustainField.style.display = Show(false);
                }
            }

            // Initial visibility update
            UpdateVisibility();

            envelopeTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            phasedTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}