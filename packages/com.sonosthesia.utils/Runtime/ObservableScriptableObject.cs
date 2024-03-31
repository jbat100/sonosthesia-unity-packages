using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public class ObservableScriptableObject : ScriptableObject
    {
        private readonly Subject<Unit> _changeSubject = new ();
        public IObservable<Unit> ChangeObservable => _changeSubject.AsObservable();

        protected void BroadcastChange() => _changeSubject.OnNext(Unit.Default);
        
        protected virtual void OnValidate() => _changeSubject.OnNext(Unit.Default);
    }
}