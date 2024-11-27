using System;
using UnityEditor;
using UnityEngine;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Mapping.Editor
{
    [CustomEditor(typeof(AbstractMapperConnection), true)]
    public class MapperConnectionBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AbstractMapperConnection connection = (AbstractMapperConnection)target;
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

        private void Perform(AbstractMapperConnection connection, string actionName, Action action)
        {
            connection.PerformUndoable(actionName + " MapperConnection", action);
        }
    }
}