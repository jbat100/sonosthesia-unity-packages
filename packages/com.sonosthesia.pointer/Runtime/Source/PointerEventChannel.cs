using Sonosthesia.Interaction;
using Sonosthesia.Channel;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public readonly struct PointerEvent : IInteractionEvent
    {
        public readonly PointerEventData Data;
        public readonly float StartTime;
        public readonly PointerSource Source;
        
        public PointerEvent(PointerEventData data, PointerSource source, float startTime)
        {
            Data = data;
            Source = source;
            StartTime = startTime;
        }

        float IInteractionEvent.StartTime => 0;
        IInteractionEndpoint IInteractionEvent.Source => Source;
        IInteractionEndpoint IInteractionEvent.Actor => null;
    }
    
    public class PointerEventChannel : Channel<PointerEvent>
    {
        
    }
}