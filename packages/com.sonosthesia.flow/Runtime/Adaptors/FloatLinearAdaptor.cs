using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class FloatLinearAdaptor<T> : SimpleFloatAdaptor<T> where T : struct
    {
        [SerializeField] private T _start;
        [SerializeField] private T _end;
        
        protected sealed override T Map(float value) => Lerp(_start, _end, value);

        protected abstract T Lerp(T start, T end, float value);
    }
}