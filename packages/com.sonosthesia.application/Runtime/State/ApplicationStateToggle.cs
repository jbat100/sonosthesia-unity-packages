using UniRx;
using UnityEngine;
using VContainer;

namespace Sonosthesia.Application
{
#if UNITY_EDITOR
    
    using UnityEditor;

    [CustomEditor(typeof(ApplicationStateToggle), true)]
    public class ApplicationStateToggleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ApplicationStateToggle toggle = (ApplicationStateToggle)target;
            if (GUILayout.Button("Toggle"))
            {
                toggle.Toggle();
            }
        }
    }
#endif
    
    public class ApplicationStateToggle : MonoBehaviour
    {
        [SerializeField] private ApplicationStateSwitchSelector _selector;
        
        private ApplicationState _state;
        
        [Inject]
        public void Construct(ApplicationState state)
        {
            _state = state;
        }

        public void Toggle()
        {
            BoolReactiveProperty property = _state.Select(_selector);

            if (property != null)
            {
                property.Value = !property.Value;
            }
        }
    }
}