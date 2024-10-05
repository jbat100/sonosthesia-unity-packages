using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TriggerableTest))]
    public class TriggerableTestEditor : Editor
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
    
    [RequireComponent(typeof(BuilderTrigger))]
    public class TriggerableTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        private BuilderTrigger _trigger;

        protected void Awake() => _trigger = GetComponent<BuilderTrigger>();

        public void Trigger()
        {
            _trigger.StartTrigger(_valueScale, _timeScale);
        }
    }
}