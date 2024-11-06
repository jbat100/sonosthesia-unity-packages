using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public abstract class HorizontalPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = UIHorizontalUtils.CreateContainer();
            Label titleLabel = UIHorizontalUtils.CreatePropertyLabel(property.name);
            container.Add(titleLabel);
            container.Add(CreateContent(property));
            return container;
        }

        protected abstract VisualElement CreateContent(SerializedProperty property);
    }
}