using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Utils.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LogRangeAttribute))]
    public class LogRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LogRangeAttribute logRangeAttribute = (LogRangeAttribute)attribute;
            LogRangeConverter rangeConverter = new LogRangeConverter(logRangeAttribute.min, logRangeAttribute.center, logRangeAttribute.max);

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

            float value = rangeConverter.ToNormalized(property.floatValue);
            value = GUI.HorizontalSlider(position, value, 0, 1);
           
            property.floatValue = rangeConverter.ToRange(value);
            EditorGUI.EndProperty();
        }
    }
#endif
}