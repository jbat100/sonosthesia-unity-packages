using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class DynamicExtractor<TEvent, TValue> : ScriptableObject, IDynamicExtractor<TEvent, TValue>
        where TValue : struct
    {
        public abstract IDynamicExtractorSession<TEvent, TValue> MakeSession();
    }
}