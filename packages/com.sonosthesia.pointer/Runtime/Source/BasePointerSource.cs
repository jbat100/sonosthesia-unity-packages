using UnityEngine;

namespace Sonosthesia.Pointer
{
    public class BasePointerSource : MonoBehaviour
    {
        [SerializeField] private PointerEventChannel _eventChannel;
        public PointerEventChannel EventChannel => _eventChannel;
    }
}