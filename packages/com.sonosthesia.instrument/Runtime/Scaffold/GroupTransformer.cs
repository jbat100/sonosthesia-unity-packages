using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Instrument
{

    public abstract class GroupTransformer : MonoBehaviour
    {
        private readonly Subject<Unit> _changeSubject = new();
        public IObservable<Unit> ChangeObservable => _changeSubject.AsObservable();

        public abstract void Apply<T>(IEnumerable<T> targets) where T : MonoBehaviour, IGroupTransformerElement;
        
        protected void BroadcastChange() => _changeSubject.OnNext(Unit.Default);
    }
}