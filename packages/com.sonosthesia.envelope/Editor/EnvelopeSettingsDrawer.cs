using Sonosthesia.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Envelope.Editor
{
    [CustomPropertyDrawer(typeof(EnvelopeSettings))]
    public class EnvelopeSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_envelopeType", 
                out SerializedProperty envelopeTypeProp, out PropertyField envelopeTypeField);
            
            root.AddRelativeField(property, "_constantValue", 
                out SerializedProperty _, out PropertyField constantValueField);
            
            root.AddRelativeField(property, "_constantDuration", 
                out SerializedProperty _, out PropertyField constantDurationField);
            
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
                // UnityEngine.Debug.Log($"{this} {nameof(UpdateVisibility)}");
                
                EnvelopeType type = (EnvelopeType)envelopeTypeProp.enumValueIndex;
                
                phasedTypeField.Show(type == EnvelopeType.Phased);
                animationCurveField.Show(type == EnvelopeType.Curve);
                envelopeFactoryField.Show(type == EnvelopeType.Custom);
                constantDurationField.Show(type == EnvelopeType.Constant);
                constantValueField.Show(type == EnvelopeType.Constant);
                
                if (type == EnvelopeType.Phased)
                {
                    PhasedEnvelopeType phasedType = (PhasedEnvelopeType)phasedTypeProp.enumValueIndex;
                    attackField.Show(phasedType is not PhasedEnvelopeType.SR);
                    decayField.Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS);
                    sustainField.Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS or PhasedEnvelopeType.SR);
                    releaseField.Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.AHR or PhasedEnvelopeType.SR);
                    holdField.Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.AHR);
                    sustainField.Show(phasedType is PhasedEnvelopeType.ADSR or PhasedEnvelopeType.ADS or PhasedEnvelopeType.SR);
                }
                else
                {
                    attackField.Show(false);
                    decayField.Show(false);
                    sustainField.Show(false);
                    releaseField.Show(false);
                    holdField.Show(false);
                    sustainField.Show(false);
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