using System;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Flow
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(UnitArpegiatorTerminatorTest))]
    public class UnitArpegiatorTerminatorTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            UnitArpegiatorTerminatorTest test = (UnitArpegiatorTerminatorTest)target;
            if (GUILayout.Button("Run"))
            {
                test.Run();
            }
        }
    }
#endif
    
    [RequireComponent(typeof(UnitArpegiatorTerminator))]
    public class UnitArpegiatorTerminatorTest : MonoBehaviour
    {
        [SerializeField] private float _duration;
        
        public void Run()
        {
            Debug.Log($"{this} run");
            GetComponent<UnitArpegiatorTerminator>()
                .Termination(Observable.Timer(TimeSpan.FromSeconds(_duration)).AsUnitObservable(), Observable.Empty<Unit>(), _duration)
                .Subscribe(_ => Debug.Log($"{this} next"), () => Debug.Log($"{this} complete"));
        }
    }
}