using System;
using UnityEditor;
using UnityEngine;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.FMOD.Editor
{
    [CustomEditor(typeof(FMODAudioController))]
    public class FMODAudioControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FMODAudioController connection = (FMODAudioController)target;
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

        private void Perform(FMODAudioController connection, string actionName, Action action)
        {
            connection.PerformUndoable(actionName + " FMODAudioController", action);
        }
    }
}