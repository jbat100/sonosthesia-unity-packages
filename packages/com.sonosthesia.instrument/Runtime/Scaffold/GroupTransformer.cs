using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Instrument
{
    [ExecuteAlways]
    public abstract class GroupTransformer : ObservableBehaviour
    {
        public abstract void Apply<T>(IEnumerable<T> targets) where T : MonoBehaviour, IGroupTransformerElement;
    }
}