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
    
    [RequireComponent(typeof(Trigger))]
    public class TriggerTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        [SerializeField] private EnvelopeFactory _envelope;
        
        private Trigger _trigger;

        protected void Awake() => _trigger = GetComponent<Trigger>();

        public void Trigger()
        {
            _trigger.TriggerController.PlayTrigger(_envelope?.Build(), _valueScale, _timeScale);
        }
    }
}