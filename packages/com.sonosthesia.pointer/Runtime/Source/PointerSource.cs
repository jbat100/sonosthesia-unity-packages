using UnityEngine;

namespace Sonosthesia.Pointer
{
    public class PointerSource : MonoBehaviour
    {
        [SerializeField] private PointerEventChannel _eventChannel;
        public PointerEventChannel EventChannel => _eventChannel;
    }
}