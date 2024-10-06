using UnityEngine;

namespace Sonosthesia.Pointer
{
    public class BasePointerSource : MonoBehaviour
    {
        [SerializeField] private PointerEventStreamContainer _eventStreamContainer;
        public PointerEventStreamContainer EventStreamContainer => _eventStreamContainer;
    }
}