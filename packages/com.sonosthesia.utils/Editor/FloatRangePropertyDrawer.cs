using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    // TODO : fix alignment issues when using the Drawer in a custom inspector
    // - the property label does not appear
    // - when created manually  
    
    //[CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create the root element
            var root = new VisualElement();

            // Create a horizontal container to hold min and max fields
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.SpaceBetween;
            // container.style.alignContent = Align.;
            // container.style.alignItems = Align.Center;

            // Add the property label
            var propertyLabel = new Label(property.displayName);
            // propertyLabel.style.minWidth = 100; // Set minimum width for the label
            // propertyLabel.style.marginRight = 10; // Add some margin to the right
            // propertyLabel.AddToClassList(FloatField.labelUssClassName);
            // propertyLabel.AddToClassList("unity-base-field__aligned");
            
            container.Add(propertyLabel);
            
            // Find the properties for min and max
            var minProperty = property.FindPropertyRelative("_min");
            var maxProperty = property.FindPropertyRelative("_max");

            // Add the property label
            // var minPropertyLabel = new Label(minProperty.displayName);
            // minPropertyLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            // root.Add(minPropertyLabel);
            
            // Create the min field and label
            var minField = new FloatField("Min");
            minField.style.flexGrow = 1;
            //minField.AddToClassList("unity-base-field__aligned");
            minField.BindProperty(minProperty);

            // Add the property label
            // var maxPropertyLabel = new Label(maxProperty.displayName);
            // maxPropertyLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            // root.Add(maxPropertyLabel);
            
            // Create the max field and label
            var maxField = new FloatField("Max");
            maxField.style.flexGrow = 1;
            //minField.AddToClassList("unity-base-field__aligned");
            maxField.BindProperty(maxProperty);

            // Add the fields to the container
            container.Add(minField);
            container.Add(maxField);

            // Add the container to the root
            root.Add(container);

            return root;
        }
    }
}