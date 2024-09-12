using System;
using FMOD.Studio;
using UniRx;
using UnityEngine;

namespace Sonosthesia.FMOD
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(FMODInstance), true)]
    public class FMODEventInstanceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FMODInstance instance = (FMODInstance)target;
            if(GUILayout.Button("Restart"))
            {
                instance.Restart();
            }
            if(GUILayout.Button("Stop"))
            {
                instance.Stop();
            }
        }
    }
#endif
    
    public abstract class FMODInstance : MonoBehaviour
    {
        private readonly BehaviorSubject<EventInstance> _instancesSubject = new (default);

        public IObservable<EventInstance> InstanceObservable => _instancesSubject.AsObservable();

        public EventInstance EventInstance
        {
            get => _instancesSubject.Value;
            protected set => _instancesSubject.OnNext(value);
        }

        public abstract void Restart();
        
        public abstract void Stop();
    }
}