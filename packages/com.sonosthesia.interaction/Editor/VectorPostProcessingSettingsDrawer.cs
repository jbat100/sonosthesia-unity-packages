using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Interaction.Editor
{
    [CustomPropertyDrawer(typeof(VectorPostProcessingSettings))]
    public class VectorPostProcessingSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.AddRelativeField(property, "_postProcessing",
                out SerializedProperty postProcessingProp, out PropertyField _);

            root.AddRelativeField(property, "_scale",
                out SerializedProperty _, out PropertyField scaleField, false);

            root.UpdateVisibility(UpdateVisibility, postProcessingProp);

            return root;

            void UpdateVisibility()
            {
                VectorProcessingType postProcessingType = (VectorProcessingType)postProcessingProp.enumValueFlag;
                scaleField.Show(postProcessingType.HasFlag(VectorProcessingType.Scale));
            }
        }
    }
}