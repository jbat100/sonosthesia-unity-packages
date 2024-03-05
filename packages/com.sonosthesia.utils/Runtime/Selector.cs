using UnityEngine;

namespace Sonosthesia.Utils
{
    public abstract class Selector<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private float _offset;

        [SerializeField] private float _scale = 1f;

        public float Select(T value) => _offset + _scale * InternalSelect(value);
        
        protected abstract float InternalSelect(T value);
    }
}