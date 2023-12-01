using UnityEditor;
using UnityEditor.AssetImporters;

namespace Sonosthesia.Timeline.MIDI
{
    // Custom inspector for MIDI file assets
    [CustomEditor(typeof(MIDIFileAssetImporter)), CanEditMultipleObjects]
    sealed class MIDIFileAssetImporterEditor : ScriptedImporterEditor
    {
        SerializedProperty _tempo;

        public override bool showImportedObject { get { return false; } }

        public override void OnEnable()
        {
            base.OnEnable();
            _tempo = serializedObject.FindProperty("_tempo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_tempo);
            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }
    }
}
