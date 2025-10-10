using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class StaticExtractor<TEvent, TValue> : ScriptableObject, IStaticExtractor<TEvent, TValue>
       where TValue : struct
    {
        public abstract bool Extract(TEvent e, out TValue value);
    }
}