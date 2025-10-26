using Sonosthesia.Interaction;
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
                MIDIExtractorType type = (MIDIExtractorType)extractorTypeProp.enumValueIndex;
                InteractionExtractorOrigin origin = (InteractionExtractorOrigin)originProp.enumValueIndex;
                
                originField.Show(type is MIDIExtractorType.Interactive or MIDIExtractorType.Provider);
                constantField.Show(type is MIDIExtractorType.Constant);
                extractorField.Show(type is MIDIExtractorType.Interactive && origin is InteractionExtractorOrigin.Self);
                providerField.Show(type is MIDIExtractorType.Provider && origin is InteractionExtractorOrigin.Self);
            }
        }
    }
}