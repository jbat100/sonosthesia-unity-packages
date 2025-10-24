using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Interaction.Editor
{
    [CustomPropertyDrawer(typeof(FloatPostProcessingSettings))]
    public class FloatPostProcessingSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.AddRelativeField(property, "_postProcessing",
                out SerializedProperty postProcessingProp, out PropertyField postProcessingField);

            root.AddRelativeField(property, "_curve",
                out SerializedProperty _, out PropertyField curveField, false);

            root.AddRelativeField(property, "_remap",
                out SerializedProperty _, out PropertyField remapField, false);

            root.AddRelativeField(property, "_clamp",
                out SerializedProperty _, out PropertyField clampField, false);

            root.UpdateVisibility(UpdateVisibility, postProcessingProp);

            return root;

            void UpdateVisibility()
            {
                FloatProcessingType postProcessingType = (FloatProcessingType)postProcessingProp.enumValueFlag;
                postProcessingType.Show(curveField, remapField, clampField);
            }
        }
    }
}