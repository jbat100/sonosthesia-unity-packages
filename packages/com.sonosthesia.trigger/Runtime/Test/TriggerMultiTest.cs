using System.Collections.Generic;
using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TriggerMultiTest))]
    public class TriggerMultiTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TriggerMultiTest test = (TriggerMultiTest)target;
            if(GUILayout.Button("Trigger"))
            {
                test.Trigger();
            }
        }
    }
#endif
    
    public class TriggerMultiTest : MonoBehaviour
    {
        [SerializeField] private float _valueScale = 1f;
        
        [SerializeField] private float _timeScale = 1f;

        [SerializeField] private EnvelopeFactory _envelope;

        [SerializeField] private List<Trigger> _signals;

        public void Trigger()
        {
            foreach (Trigger trigger in _signals)
            {
                trigger.TriggerController.PlayTrigger(_envelope?.Build(), _valueScale, _timeScale);
            }
        }
    }
}