using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Instrument.Editor
{
    [CustomPropertyDrawer(typeof(MIDIChannelTouchExtractorSettings))]
    public class MIDIChannelTouchExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.displayName);
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_extractorType", 
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);
            
            root.AddRelativeField(property, "_channel", 
                out SerializedProperty _, out PropertyField channelField);
            
            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField);

            void UpdateVisibility()
            {
                MIDIChannelTouchExtractorSettings.ExtractorType type 
                    = (MIDIChannelTouchExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                
                channelField.Show(type == MIDIChannelTouchExtractorSettings.ExtractorType.Static);
                extractorField.Show(type == MIDIChannelTouchExtractorSettings.ExtractorType.Custom);
            }

            UpdateVisibility();

            extractorTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}