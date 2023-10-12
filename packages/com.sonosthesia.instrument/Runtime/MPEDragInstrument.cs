using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class MPEDragInstrument : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private int _note;
        
        [SerializeField] private int _velocity;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}