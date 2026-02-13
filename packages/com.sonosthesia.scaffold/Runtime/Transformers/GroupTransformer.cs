using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Scaffold
{
    [ExecuteAlways]
    public abstract class GroupTransformer : ObservableBehaviour
    {
        public abstract void Apply<T>(IReadOnlyList<T> targets) where T : MonoBehaviour, IGroupTransformerElement;
    }
}