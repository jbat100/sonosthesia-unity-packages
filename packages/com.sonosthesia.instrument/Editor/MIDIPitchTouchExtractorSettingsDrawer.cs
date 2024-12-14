using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Instrument.Editor
{
    [CustomPropertyDrawer(typeof(MIDIPitchTouchExtractorSettings), true)]
    public class MIDIPitchTouchExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.displayName);
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_extractorType", 
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);
            
            root.AddRelativeField(property, "_value", 
                out SerializedProperty _, out PropertyField valueField);
            
            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField);

            void UpdateVisibility()
            {
                MIDIPitchTouchExtractorSettings.ExtractorType type 
                    = (MIDIPitchTouchExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                
                valueField.Show(type == MIDIPitchTouchExtractorSettings.ExtractorType.Static);
                extractorField.Show(type == MIDIPitchTouchExtractorSettings.ExtractorType.Custom);
            }

            UpdateVisibility();

            extractorTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}