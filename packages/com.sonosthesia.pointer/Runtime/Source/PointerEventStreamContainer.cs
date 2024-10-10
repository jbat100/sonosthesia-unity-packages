using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public readonly struct PointerEvent : IInteractionEvent
    {
        public readonly PointerEventData Data;
        
        public PointerEvent(PointerEventData data)
        {
            Data = data;
        }

        public IInteractionEndpoint Source => null;
        public IInteractionEndpoint Actor => null;
    }
    
    public class PointerEventStreamContainer : StreamContainer<PointerEvent>
    {
        
    }
}