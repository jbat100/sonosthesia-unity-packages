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

        protected virtual void OnEnable() => BroadcastChange();

        public void BroadcastChange()
        {
            OnChanged();
            // Debug.Log($"{this} {nameof(BroadcastChange)}");
            _changeSubject.OnNext(Unit.Default);
        }

        protected virtual void OnChanged()
        {
            
        }

        protected void BroadcastSet<T>(T updated, ref T value)
        {
            TriggerSet.Set(updated, ref value, BroadcastChange);
        }
    }
}