using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonosthesia.Mapping
{
#if UNITY_EDITOR
    
    using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(MapperConnectionBase), true)]
    public class MapperConnectionBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapperConnectionBase connection = (MapperConnectionBase)target;
            if (GUILayout.Button("Autofill"))
            {
                Perform(connection, "Autofill", () => connection.AutofillSlots(false));
            }
            if (GUILayout.Button("Recursive Autofill"))
            {
                Perform(connection, "Recursive Autofill", () => connection.AutofillSlots(true));
            }
            if (GUILayout.Button("Clear"))
            {
                Perform(connection, "Clear", () => connection.DeleteAllSlots());
            }
        }

        private void Perform(MapperConnectionBase connection, string actionName, Action action)
        {
            Undo.RecordObject(connection, actionName + " MapperConnection");
            action();
            PrefabUtility.RecordPrefabInstancePropertyModifications(connection);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
#endif
    
    public abstract class MapperConnectionBase : MonoBehaviour
    {
        public abstract void AutofillSlots(bool recursive);
        
        public abstract void DeleteAllSlots();
    }
}