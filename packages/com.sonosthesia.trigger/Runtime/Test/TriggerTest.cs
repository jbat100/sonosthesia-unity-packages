using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TriggerTest))]
    public class TriggerTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TriggerTest test = (TriggerTest)target;
            if(GUILayout.Button("Trigger"))
            {
                test.Trigger();
            }
        }
    }
#endif
    
    public class TriggerTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        [SerializeField] private EnvelopeFactory _envelope;
        
        [SerializeField] private Trigger _trigger;

        protected void Awake()
        {
            if (!_trigger)
            {
                _trigger = GetComponent<Trigger>();
            }
        } 

        public void Trigger()
        {
            _trigger.TriggerImplementation.StartTrigger(_envelope.Build(), _valueScale, _timeScale, true);
        }
        
        public void Trigger(float valueScale)
        {
            _trigger.TriggerImplementation.StartTrigger(_envelope.Build(), _valueScale * valueScale, _timeScale, true);
        }
    }
}