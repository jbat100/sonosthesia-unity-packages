using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class FloatLinearAdaptor<T> : FloatMapAdaptor<T> where T : struct
    {
        [SerializeField] private T _start;
        [SerializeField] private T _end;
        
        protected sealed override T Map(float value) => Lerp(_start, _end, value);

        protected abstract T Lerp(T start, T end, float value);
    }
}