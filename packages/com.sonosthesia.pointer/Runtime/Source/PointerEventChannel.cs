using Sonosthesia.Interaction;
using Sonosthesia.Channel;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public readonly struct PointerEvent : IInteractionEvent
    {
        public readonly PointerEventData Data;
        public readonly float StartTime;
        
        public PointerEvent(PointerEventData data, float startTime)
        {
            Data = data;
            StartTime = startTime;
        }

        float IInteractionEvent.StartTime => 0;
        public IInteractionEndpoint Source => null;
        public IInteractionEndpoint Actor => null;
    }
    
    public class PointerEventChannel : Channel<PointerEvent>
    {
        
    }
}