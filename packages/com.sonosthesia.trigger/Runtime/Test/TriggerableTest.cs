using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TriggerableTest))]
    public class TriggerFloatSignalTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TriggerableTest test = (TriggerableTest)target;
            if(GUILayout.Button("Trigger"))
            {
                test.Trigger();
            }
        }
    }
#endif
    
    [RequireComponent(typeof(Triggerable))]
    public class TriggerableTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        private Triggerable _signal;

        protected void Awake() => _signal = GetComponent<Triggerable>();

        public void Trigger()
        {
            _signal.Trigger(_valueScale, _timeScale);
        }
    }
}