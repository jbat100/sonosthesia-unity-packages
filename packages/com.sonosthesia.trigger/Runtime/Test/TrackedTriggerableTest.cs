using System;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TrackedTriggerableTest))]
    public class TrackedTriggerableTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TrackedTriggerableTest test = (TrackedTriggerableTest)target;
            if(GUILayout.Button("Start"))
            {
                test.StartTrigger();
            }
            if(GUILayout.Button("End"))
            {
                test.EndTrigger();
            }
        }
    }
#endif
    
    [RequireComponent(typeof(BuilderTrackedTrigger))]
    public class TrackedTriggerableTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        private BuilderTrackedTrigger _trigger;
        private Guid _current;

        protected virtual void Awake()
        {
            _trigger = GetComponent<BuilderTrackedTrigger>();
        }

        public void StartTrigger() => StartTrigger(_valueScale, _timeScale);

        public void StartTrigger(float valueScale, float timeScale)
        {
            if (_current != Guid.Empty)
            {
                EndTrigger(_timeScale);
            }
            
            _current = _trigger.StartTrigger(valueScale, timeScale);
        }

        public void EndTrigger() => EndTrigger(_timeScale);

        public void EndTrigger(float timeScale)
        {
            if (_current == Guid.Empty)
            {
                return;
            }

            _trigger.EndTrigger(_current, timeScale);
            
            _current = Guid.Empty;
        }
    }
}