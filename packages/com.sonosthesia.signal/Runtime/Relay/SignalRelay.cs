using System;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class SignalRelay<TValue> : ScriptableObject where TValue : struct
    {
        public abstract void Broadcast(TValue value);

        public abstract IObservable<TValue> Observable { get; }
    }
}