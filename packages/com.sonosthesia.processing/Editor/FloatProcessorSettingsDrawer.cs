using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Processing.Editor
{
    [CustomPropertyDrawer(typeof(FloatProcessorSettings))]
    public class FloatProcessorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.AddRelativeField(property, "_processing",
                out SerializedProperty processingProp, out PropertyField _);

            root.AddRelativeField(property, "_curve",
                out SerializedProperty _, out PropertyField curveField, false);

            root.AddRelativeField(property, "_remap",
                out SerializedProperty _, out PropertyField remapField, false);

            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField, false);
            
            root.AddRelativeField(property, "_randomization",
                out SerializedProperty _, out PropertyField randomizationField, false);

            root.UpdateVisibility(UpdateVisibility, processingProp);

            return root;

            void UpdateVisibility()
            {
                FloatProcessingType postProcessingType = (FloatProcessingType)processingProp.enumValueFlag;
                postProcessingType.Show(curveField, remapField, clampField, randomizationField);
            }
        }
    }
}