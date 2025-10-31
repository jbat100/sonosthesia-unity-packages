using System;
using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TrackedTriggerTest))]
    public class TrackedTriggerTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TrackedTriggerTest test = (TrackedTriggerTest)target;
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
    
    public class TrackedTriggerTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        [SerializeField] private EnvelopeFactory _startEnvelope;
        
        [SerializeField] private EnvelopeFactory _endEnvelope;
        
        [SerializeField] private Trigger _trigger;
        
        private Guid _current;

        protected virtual void Awake()
        {
            if (!_trigger)
            {
                _trigger = GetComponent<Trigger>();
            }
        }

        public void StartTrigger() => StartTrigger(_valueScale, _timeScale);

        public void StartTrigger(float valueScale, float timeScale)
        {
            if (_current != Guid.Empty)
            {
                EndTrigger(timeScale);
            }
            IEnvelope envelope = _startEnvelope.Build();
            _current = _trigger.TriggerController.StartTrigger(envelope, valueScale * _valueScale, timeScale * _timeScale);
        }

        public void EndTrigger() => EndTrigger(_timeScale);

        public void EndTrigger(float timeScale)
        {
            if (_current == Guid.Empty)
            {
                return;
            }
            IEnvelope envelope = _endEnvelope.Build();
            _trigger.TriggerController.EndTrigger(_current, envelope, timeScale);
            _current = Guid.Empty;
        }
    }
}