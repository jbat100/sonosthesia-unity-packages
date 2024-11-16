using Sonosthesia.Interaction;
using Sonosthesia.Channel;
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

        public float StartTime => 0;
        public IInteractionEndpoint Source => null;
        public IInteractionEndpoint Actor => null;
    }
    
    public class PointerEventChannel : Channel<PointerEvent>
    {
        
    }
}