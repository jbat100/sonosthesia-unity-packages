using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class ValueProvider<T> : MonoBehaviour where T : struct
    {
        public abstract T Value { get; }
    }
}