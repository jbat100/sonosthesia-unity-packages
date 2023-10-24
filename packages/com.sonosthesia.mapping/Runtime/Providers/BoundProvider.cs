using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class BoundProvider<T> : ValueProvider<T> where T : struct
    {
        [SerializeField] private T _lower;
        [SerializeField] private T _upper;

        protected abstract T Randomize(T lower, T upper);

        public override T Value => Randomize(_lower, _upper);
    }
}