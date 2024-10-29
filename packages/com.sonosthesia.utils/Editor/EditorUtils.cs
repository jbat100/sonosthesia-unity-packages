using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Sonosthesia.Utils.Editor
{
    public static class EditorUtils
    {
        public static void PerformUndoable(this UnityEngine.Object connection, string actionName, Action action)
        {
            Undo.RecordObject(connection, actionName);
            action();
            PrefabUtility.RecordPrefabInstancePropertyModifications(connection);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}