using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class Extractor<TEvent, TValue> : ScriptableObject 
        where TEvent : IInteractionEvent where TValue : struct
    {
        public abstract IExtractorSession<TEvent, TValue> MakeSession();
    }
}