using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    // note : for use with the new input system the main camera needs to have a PhysicsRaycaster
    public class PointerTouchDriver : EventTouchDriver, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnPointerDown)}");
            Begin(MakePayload(eventData));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnPointerUp)}");
            Clean();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            //Debug.Log($"{this} {nameof(OnPointerMove)}");
            if (ShouldMove(eventData))
            {
                Move(MakePayload(eventData));
            }
        }
    }
}