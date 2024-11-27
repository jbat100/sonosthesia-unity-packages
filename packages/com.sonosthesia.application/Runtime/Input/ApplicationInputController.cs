using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Sonosthesia.Application
{
    public class ApplicationInputController : MonoBehaviour
    {
        [SerializeField] private InputActionProperty _menu;

        private ApplicationState _state;
        
        [Inject]
        public void Construct(ApplicationState state)
        {
            _state = state;
        }
        
        protected void OnEnable()
        {
            if (_menu.action != null)
            {
                _menu.action.performed += OnMenu;
            }
        }

        protected void OnDisable()
        {
            if (_menu.action != null)
            {
                _menu.action.performed -= OnMenu;
            }
        }

        private void OnMenu(InputAction.CallbackContext obj)
        {
            _state.activeUI.Toggle();
        }
    }
}