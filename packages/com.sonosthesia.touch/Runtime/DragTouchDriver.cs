using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    // note : for use with the new input system the main camera needs to have a PhysicsRaycaster
    public class DragTouchDriver : TouchDriver, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnBeginDrag)}");
            Begin(MakePayload(eventData));
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnDrag)}");
            if (ShouldMove(eventData))
            {
                Move(MakePayload(eventData));
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnEndDrag)}");
            Clean();
        }
    }
}