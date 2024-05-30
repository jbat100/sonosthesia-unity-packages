using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Utils
{
    /// <summary>
    /// Cheap way of avoiding singleton while not going for full fledged DI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScriptableSlot<T> : ScriptableObject where T : Component
    {
        private readonly BehaviorSubject<T> _subject = new(null);
        public IObservable<T> Observable => _subject.AsObservable();
        public T Value
        {
            get => _subject.Value;
            set => _subject.OnNext(value);
        }
    }
}