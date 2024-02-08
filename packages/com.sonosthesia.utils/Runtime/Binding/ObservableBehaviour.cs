using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public class ObservableBehaviour : MonoBehaviour
    {
        private readonly Subject<Unit> _changeSubject = new();
        public IObservable<Unit> ChangeObservable => _changeSubject.AsObservable();

        protected virtual void OnValidate() => BroadcastChange();

        protected void BroadcastChange() => _changeSubject.OnNext(Unit.Default);

        protected void BroadcastSet<T>(T updated, ref T value)
        {
            TriggerSet.Set(updated, ref value, BroadcastChange);
        }
    }
}