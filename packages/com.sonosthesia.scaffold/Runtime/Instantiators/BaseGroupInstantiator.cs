using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BaseGroupInstantiator), true)]
    public class BaseGroupInstantiatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseGroupInstantiator intantiator = (BaseGroupInstantiator)target;
            if(GUILayout.Button("Reload"))
            {
                intantiator.Reload();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
#endif
    
    public abstract class BaseGroupInstantiator : MonoBehaviour, IObjectGroup
    {
        private readonly Subject<Unit> _objectsChangedSubject = new ();
        public IObservable<Unit> ObjectsChangedObservable => _objectsChangedSubject.AsObservable();
        
        public abstract void Reload();
        
        public abstract IEnumerable<GameObject> Objects { get; }
        
        protected void NotifyObjectsChanged() => _objectsChangedSubject.OnNext(Unit.Default);
    }
}