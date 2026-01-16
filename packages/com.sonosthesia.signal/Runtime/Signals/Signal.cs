using System;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public interface ISignal<T> where T : struct
    {
        IObservable<T> Observable { get; }
        
        void Broadcast(T value);
    }

    public abstract class Signal<T> : MonoBehaviour, ISignal<T> where T : struct
    {
        public abstract IObservable<T> Observable { get; }

        public abstract void Broadcast(T value);
    }
}