using UnityEngine;

namespace Sonosthesia.Mapping
{
    // move to a separate provider package
    
    public class StaticProvider<T> : ValueProvider<T> where T : struct
    {
        [SerializeField] private T _value;

        public override T Value => _value;
    }
}