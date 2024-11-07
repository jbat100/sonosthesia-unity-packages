using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public abstract class HorizontalPropertyDrawer : PropertyDrawer
    {
        protected virtual VisualElement CreatePropertyContainer() => UIHorizontalUtils.CreatePropertyContainer();
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = UIHorizontalUtils.CreateContainer();
            Label titleLabel = UIHorizontalUtils.CreatePropertyLabel(property.name);
            container.Add(titleLabel);
            VisualElement propertyContainer = CreatePropertyContainer();
            CreateContent(property, propertyContainer);
            container.Add(propertyContainer);
            return container;
        }

        protected abstract void CreateContent(SerializedProperty property, VisualElement propertyContainer);
    }
}