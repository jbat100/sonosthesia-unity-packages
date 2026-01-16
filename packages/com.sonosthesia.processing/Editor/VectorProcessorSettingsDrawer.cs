using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Processing.Editor
{
    [CustomPropertyDrawer(typeof(VectorProcessorSettings))]
    public class VectorProcessorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.AddRelativeField(property, "_processor",
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