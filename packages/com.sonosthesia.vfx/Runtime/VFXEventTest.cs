using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.VFX
{
    
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(VFXEventTest), true)]
    public class VFXEventTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VFXEventTest test = (VFXEventTest)target;
            if(GUILayout.Button("Send"))
            {
                test.Send();
            }
        }
    }
#endif
    
    public class VFXEventTest : MonoBehaviour
    {
        [SerializeField] private VisualEffect _visualEffect;
        
        [SerializeField] private string _eventName;

        public void Send()
        {
            VFXEventAttribute eventAttribute = _visualEffect.CreateVFXEventAttribute();
            ConfigureEventAttribute(eventAttribute);
            _visualEffect.SendEvent(_eventName, eventAttribute);
        }

        protected virtual void ConfigureEventAttribute(VFXEventAttribute eventAttribute)
        {
            
        }
    }
}