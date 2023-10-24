using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class LinearFader<T> : Fader<T> where T : struct
    {
        [SerializeField] private T _start;

        [SerializeField] private T _end;
        
        public override T Fade(float fade)
        {
            return Lerp(_start, _end, fade);
        }

        protected abstract T Lerp(T start, T end, float value);
    }
}