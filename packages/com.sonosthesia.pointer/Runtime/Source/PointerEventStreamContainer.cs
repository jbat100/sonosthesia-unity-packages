using Sonosthesia.Utils;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public readonly struct PointerEvent
    {
        public readonly PointerEventData Data;

        public PointerEvent(PointerEventData data)
        {
            Data = data;
        }
    }
    
    public class PointerEventStreamContainer : StreamContainer<PointerEvent>
    {
        
    }
}