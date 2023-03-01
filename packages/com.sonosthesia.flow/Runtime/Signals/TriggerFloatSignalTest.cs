using UnityEngine;

namespace Sonosthesia.Flow
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TriggerFloatSignalTest))]
    public class TriggerFloatSignalTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TriggerFloatSignalTest test = (TriggerFloatSignalTest)target;
            if(GUILayout.Button("Trigger"))
            {
                test.Trigger();
            }
        }
    }
#endif
    
    [RequireComponent(typeof(TriggerFloatSignal))]
    public class TriggerFloatSignalTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        private TriggerFloatSignal _signal;

        protected void Awake() => _signal = GetComponent<TriggerFloatSignal>();

        public void Trigger()
        {
            _signal.Trigger(_valueScale, _timeScale);
        }
    }
}