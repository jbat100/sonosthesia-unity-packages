using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class SelectorAdaptor<TValue> : MapAdaptor<TValue, float> where TValue : struct
    {
        [SerializeField] private Selector<TValue> _selector;

        protected override float Map(TValue value) => _selector.Select(value);
    }
}