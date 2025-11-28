using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class Mixer<TValue> : StatelessSignal<TValue> where TValue : struct
    {
        [SerializeField] private List<InterfaceReference<IStatefulSignal<TValue>>> _inputs;

        private static readonly List<TValue> _values = new ();
        
        protected abstract TValue Mix(IEnumerable<TValue> values);
        
        protected virtual void Update()
        {
            _values.Clear();
            foreach (InterfaceReference<IStatefulSignal<TValue>> input in _inputs)
            {
                if (input.Value != null)
                {
                    _values.Add(input.Value.Value);
                }
            }
            Broadcast(Mix(_values));
        }
        
    }
}