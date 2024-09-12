using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Sonosthesia.Application
{
    public class PointerEvents : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public UnityEvent onUp;

        public UnityEvent onDown;
        
        public void OnPointerUp(PointerEventData eventData)
        {
            onUp.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onDown.Invoke();
        }
    }
}