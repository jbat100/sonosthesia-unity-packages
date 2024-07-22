using UnityEngine;

namespace Sonosthesia.Utils
{
    public class RegisterItem : MonoBehaviour
    {
        [SerializeField] private RegisterScriptableObject<RegisterItem> _register;

        protected virtual void OnEnable()
        {
            if (_register)
            {
                _register.Register(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (_register)
            {
                _register.Unregister(this);
            }
        }
    }
}