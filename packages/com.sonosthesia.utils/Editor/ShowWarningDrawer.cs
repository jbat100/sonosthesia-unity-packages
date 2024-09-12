using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ShowWarningAttribute))]
    public class ShowWarningDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowWarningAttribute warning = (ShowWarningAttribute)attribute;
            EditorGUI.HelpBox(position, warning.Message, MessageType.Warning);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}