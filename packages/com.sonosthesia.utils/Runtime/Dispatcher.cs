using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    
#if UNITY_EDITOR
    
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    [CustomEditor(typeof(Dispatcher), true)]
    public class DispatcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Dispatcher dispatcher = (Dispatcher)target;
            if (GUILayout.Button("Autofill"))
            {
                Perform(dispatcher, "Autofill", () => dispatcher.AutofillDestinations());
            }
            if (GUILayout.Button("Clear"))
            {
                Perform(dispatcher, "Clear", () => dispatcher.DeleteAllDestinations());
            }
        }

        private void Perform(Dispatcher connection, string actionName, Action action)
        {
            Undo.RecordObject(connection, actionName + " Dispatcher");
            action();
            PrefabUtility.RecordPrefabInstancePropertyModifications(connection);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
#endif
    
    public abstract class Dispatcher : MonoBehaviour
    {
        public enum Mode
        {
            Sequential,
            Random,
            Mixed
        }

        [SerializeField] private Mode _mode;

        [SerializeField] private float _randomization;
        
        protected abstract int DestinationCount { get; }
        
        private int? _lastIndex;
        
#if UNITY_EDITOR
        public abstract void AutofillDestinations();
        
        public abstract void DeleteAllDestinations();
#endif

        protected virtual void OnEnable()
        {
            _lastIndex = null;
        }
        
        protected virtual void OnDisable()
        {
            _lastIndex = null;
        }
        
        protected int StepIndex()
        {
            _lastIndex = _mode switch
            {
                Mode.Sequential => _lastIndex.HasValue ? (_lastIndex.Value + 1) % DestinationCount : 0,
                Mode.Mixed => _lastIndex.HasValue ? (_lastIndex.Value + 1) % DestinationCount : 0,
                Mode.Random => UnityEngine.Random.Range(0, DestinationCount),
                _ => 0
            };

            if (_mode == Mode.Mixed)
            {
                return (_lastIndex.Value + UnityEngine.Random.Range(0, Mathf.FloorToInt(DestinationCount * _randomization))) % DestinationCount;
            }

            return _lastIndex.Value;
        }
    }
}