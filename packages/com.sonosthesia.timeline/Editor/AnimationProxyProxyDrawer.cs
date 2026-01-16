using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Timeline.Editor
{
    [CustomPropertyDrawer(typeof(AnimationProxy.Proxy))]
    public class AnimationProxyProxyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Force a single line
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin the property
            EditorGUI.BeginProperty(position, label, property);

            // Draw the label without a foldout
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // No indent inside the line
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Find the fields
            SerializedProperty valueProp  = property.FindPropertyRelative("value");
            SerializedProperty signalProp = property.FindPropertyRelative("signal");

            // Define rects
            const float valueWidth = 50f;          // small width for the float
            const float spacing    = 4f;

            Rect valueRect  = new Rect(position.x, position.y, valueWidth, position.height);
            Rect signalRect = new Rect(position.x + valueWidth + spacing, position.y,
                position.width - valueWidth - spacing, position.height);

            // Draw fields (no sub-labels)
            EditorGUI.PropertyField(valueRect,  valueProp,  GUIContent.none);
            EditorGUI.PropertyField(signalRect, signalProp, GUIContent.none);

            // Restore indent and end
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}