using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class ArpegiatorTerminator<T> : MonoBehaviour where T: struct
    {
        // Observable completes when termination is required
        public abstract IObservable<Unit> Termination(IObservable<T> original, IObservable<T> arpegiated, float offset);
    }
}