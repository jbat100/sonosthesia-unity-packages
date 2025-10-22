using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Instrument.Editor
{
    public class MIDIExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.displayName);
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_extractorType", 
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);
            
            root.AddRelativeField(property, "_origin", 
                out SerializedProperty originProp, out PropertyField originField);
            
            root.AddRelativeField(property, "_constant", 
                out SerializedProperty _, out PropertyField constantField);
            
            root.AddRelativeField(property, "_extractor", 
                out SerializedProperty _, out PropertyField extractorField);

            root.AddRelativeField(property, "_provider", 
                out SerializedProperty _, out PropertyField providerField);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp, originProp);
            
            return root;

            void UpdateVisibility()
            {
                MIDIExtractorSettings.ExtractorType type 
                    = (MIDIExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;
                
                MIDIExtractorSettings.Origin origin 
                    = (MIDIExtractorSettings.Origin)originProp.enumValueIndex;
                
                originField.Show(type is MIDIExtractorSettings.ExtractorType.Interactive or MIDIExtractorSettings.ExtractorType.Provider);
                constantField.Show(type is MIDIExtractorSettings.ExtractorType.Constant);
                extractorField.Show(type is MIDIExtractorSettings.ExtractorType.Interactive && origin is MIDIExtractorSettings.Origin.Self);
                providerField.Show(type is MIDIExtractorSettings.ExtractorType.Provider && origin is MIDIExtractorSettings.Origin.Self);
            }
        }
    }
}