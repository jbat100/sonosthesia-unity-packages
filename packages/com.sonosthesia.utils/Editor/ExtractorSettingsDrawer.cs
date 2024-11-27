using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ExtractorSettings<,>), true)]
    public class ExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.displayName);
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_selector");
            
            root.AddRelativeField(property, "_postProcessing", 
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);
            
            root.AddRelativeField(property, "_remap", 
                out SerializedProperty _, out PropertyField remapField);
            
            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField);
            
            root.AddRelativeField(property, "_randomize", 
                out SerializedProperty _, out PropertyField randomizeField);
            
            void UpdateVisibility()
            {
                ExtractorPostProcessingType postProcessingType = (ExtractorPostProcessingType)postProcessingProp.enumValueFlag;
                
                remapField.Show(postProcessingType.HasFlag(ExtractorPostProcessingType.Remap));
                clampField.Show(postProcessingType.HasFlag(ExtractorPostProcessingType.Clamp));
                randomizeField.Show(postProcessingType.HasFlag(ExtractorPostProcessingType.Randomize));
            }

            UpdateVisibility();
            
            postProcessingField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}