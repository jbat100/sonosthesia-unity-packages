using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class LinearFader<T> : Fader<T> where T : struct
    {
        [SerializeField] private T _start;

        [SerializeField] private T _end;

        [SerializeField] private bool _clamp;
        
        public override T Fade(float fade)
        {
            return LerpUnclamped(_start, _end, _clamp ? Mathf.Clamp01(fade) : fade);
        }

        protected abstract T LerpUnclamped(T start, T end, float value);
    }
}