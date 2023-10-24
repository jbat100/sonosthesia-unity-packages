using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class ValueProvider<T> : MonoBehaviour where T : struct
    {
        public abstract T Value { get; }
    }
}