using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Interaction.Editor
{
    [CustomPropertyDrawer(typeof(IInteractiveTriggerSettings<>), true)]
    public class InteractiveTriggerSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            root.Add(UIElementUtils.Separator());
            
            root.Add(UIElementUtils.SectionLabel(property.name.PropertyNameToLabel()));
            
            root.AddRelativeField(property, "_interaction", 
                out SerializedProperty typeProp, out PropertyField _);
            
            root.AddRelativeField(property, "_valueExtractor", 
                out SerializedProperty _, out PropertyField _);
            
            root.AddRelativeField(property, "_attackExtractor", 
                out SerializedProperty _, out PropertyField _);
            
            root.AddRelativeField(property, "_envelope", 
                out SerializedProperty _, out PropertyField _);

            root.AddRelativeField(property, "_releaseExtractor", 
                out SerializedProperty _, out PropertyField releaseExtractorField);
            
            root.AddRelativeField(property, "_releaseType", 
                out SerializedProperty _, out PropertyField releaseTypeField);

            root.UpdateVisibility(UpdateVisibility, typeProp);
            
            return root;

            void UpdateVisibility()
            {
                TriggerInteraction type = (TriggerInteraction)typeProp.enumValueIndex;
                releaseExtractorField.Show(type is TriggerInteraction.Hold);
                releaseTypeField.Show(type is TriggerInteraction.Hold);
            }
        }
    }
}