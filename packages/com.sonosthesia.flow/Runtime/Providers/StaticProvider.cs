using UnityEngine;

namespace Sonosthesia.Flow
{
    public class StaticProvider<T> : ValueProvider<T> where T : struct
    {
        [SerializeField] private T _value;

        public override T Value => _value;
    }
}