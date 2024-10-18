using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Touch.Editor
{
    [CustomPropertyDrawer(typeof(TouchEnvelopeSettings))]
    public class TouchEnvelopeSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_type", 
                out SerializedProperty typeProp, out PropertyField typeField);
            
            root.AddRelativeField(property, "_constantExtractor", 
                out SerializedProperty _, out PropertyField constantExtractorField);
            
            root.AddRelativeField(property, "_valueScaleExtractor", 
                out SerializedProperty _, out PropertyField valueScaleExtractorField);
            
            root.AddRelativeField(property, "_timeScaleExtractor", 
                out SerializedProperty _, out PropertyField timeScaleExtractorField);
            
            root.AddRelativeField(property, "_envelope", 
                out SerializedProperty _, out PropertyField envelopeField);
            
            root.AddRelativeField(property, "_trackValue", 
                out SerializedProperty _, out PropertyField trackValueField);
            
            root.AddRelativeField(property, "_releaseExtractor", 
                out SerializedProperty _, out PropertyField releaseExtractorField);
            
            root.AddRelativeField(property, "_releaseType", 
                out SerializedProperty _, out PropertyField releaseTypeField);
            
            void UpdateVisibility()
            {
                TouchEnvelopeSettings.TouchType type 
                    = (TouchEnvelopeSettings.TouchType)typeProp.enumValueIndex;
                
                constantExtractorField.Show(type is TouchEnvelopeSettings.TouchType.Constant);
                
                valueScaleExtractorField.Show(type is not TouchEnvelopeSettings.TouchType.Constant);
                timeScaleExtractorField.Show(type is not TouchEnvelopeSettings.TouchType.Constant);
                envelopeField.Show(type is not TouchEnvelopeSettings.TouchType.Constant);
                trackValueField.Show(type is not TouchEnvelopeSettings.TouchType.Constant);
                
                releaseExtractorField.Show(type is TouchEnvelopeSettings.TouchType.Contact);
                releaseTypeField.Show(type is TouchEnvelopeSettings.TouchType.Contact);
            }

            UpdateVisibility();
            typeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}