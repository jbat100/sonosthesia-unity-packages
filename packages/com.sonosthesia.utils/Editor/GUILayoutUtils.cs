using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Utils.Editor
{
    public static class GUILayoutUtils
    {
        public static void DrawSeparator()
        {
            GUILayout.Space(10); // Add some space before the separator
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(10); // Add some space after the separator
        }
    }
}