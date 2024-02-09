using UnityEngine;
using UnityEngine.Pool;

namespace Sonosthesia.Utils
{
    public abstract class ScriptablePool<T> : ScriptableObject where T : class
    {
        public abstract IObjectPool<T> Pool { get; }
    }
}