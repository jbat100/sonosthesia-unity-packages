using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangePropertyDrawer : HorizontalPropertyDrawer
    {
        protected override void CreateContent(SerializedProperty property, VisualElement propertyContainer)
        {
            SerializedProperty minProp = property.FindPropertyRelative("_min");
            SerializedProperty maxProp = property.FindPropertyRelative("_max");

            Label minLabel = UIHorizontalUtils.CreateLabel("Min", 30);
            FloatField minField = UIHorizontalUtils.CreateFloatField(minProp);
            Label maxLabel = UIHorizontalUtils.CreateLabel("Max", 30);
            FloatField maxField = UIHorizontalUtils.CreateFloatField(maxProp);
            
            propertyContainer.Add(minLabel);
            propertyContainer.Add(minField);
            propertyContainer.Add(maxLabel);
            propertyContainer.Add(maxField);
        }
    }
}