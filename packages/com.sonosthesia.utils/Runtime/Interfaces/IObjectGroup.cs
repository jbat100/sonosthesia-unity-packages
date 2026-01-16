using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public interface IObjectGroup
    {
        IEnumerable<GameObject> Objects { get; }
        
        IObservable<Unit> ObjectsChangedObservable { get; }
    }
}