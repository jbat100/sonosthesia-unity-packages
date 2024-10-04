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

        [SerializeField] private List<BuilderTriggerable> _signals;

        public void Trigger()
        {
            foreach (BuilderTriggerable triggerable in _signals)
            {
                triggerable.Trigger(_valueScale, _timeScale);
            }
        }
    }
}