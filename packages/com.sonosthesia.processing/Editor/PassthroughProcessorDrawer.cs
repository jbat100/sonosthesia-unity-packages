using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Processing.Editor
{
    [CustomPropertyDrawer(typeof(PassthroughProcessor<>))]
    public class PassthroughProcessorDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            return root;
        }
    }
}