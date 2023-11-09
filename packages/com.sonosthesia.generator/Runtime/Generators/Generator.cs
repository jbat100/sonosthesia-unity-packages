using UnityEngine;

namespace Sonosthesia.Generator
{
    public abstract class Generator<T> : MonoBehaviour where T : struct
    {
        public abstract T Evaluate(float time);
    }
}