using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TriggerableMultiTest))]
    public class TriggerableMultiTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TriggerableMultiTest test = (TriggerableMultiTest)target;
            if(GUILayout.Button("Trigger"))
            {
                test.Trigger();
            }
        }
    }
#endif
    
    public class TriggerableMultiTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        [SerializeField] private List<BuilderTrigger> _signals;

        public void Trigger()
        {
            foreach (BuilderTrigger triggerable in _signals)
            {
                triggerable.StartTrigger(_valueScale, _timeScale);
            }
        }
    }
}