using UnityEngine;

namespace Sonosthesia.Utils
{
    public class ScriptableSlotRegister<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private T _value;
        
        [SerializeField] private ScriptableSlot<T> _slot;

        // caching current prevents loss if _value is changed between OnEnable and OnDisable
        private T _current;
        
        protected virtual void OnEnable()
        {
            if (_slot)
            {
                _current = _value;
                _slot.Value = _value;
            }
        }

        protected virtual void OnDisable()
        {
            if (_slot && _slot.Value == _current)
            {
                _current = null;
                _slot.Value = null;
            }
        }
    }
}