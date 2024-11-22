using UnityEngine;
using UnityEngine.InputSystem;

namespace Sonosthesia.XR
{
    // https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.13/manual/features/oculustouchcontrollerprofile.html
    
    public class ControllerTest : MonoBehaviour
    {
        [SerializeField]
        InputActionProperty _axis = new InputActionProperty(new InputAction("Grip", expectedControlType: "Axis"));
        
        protected virtual void Update()
        {
            float axis = _axis.action.ReadValue<float>();
            
            Debug.Log($"{this} {nameof(axis)} {axis}");
        }
        
    }
}
