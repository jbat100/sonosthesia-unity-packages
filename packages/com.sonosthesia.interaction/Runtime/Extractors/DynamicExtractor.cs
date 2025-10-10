using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class DynamicExtractor<TEvent, TValue> : ScriptableObject 
        where TEvent : IInteractionEvent where TValue : struct
    {
        public abstract IExtractorSession<TEvent, TValue> MakeSession();
    }
}