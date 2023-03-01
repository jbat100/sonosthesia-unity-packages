using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Spawn
{

#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(TestVFXEvent))]
    public class TestEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TestVFXEvent testVFXEvent = (TestVFXEvent)target;
            if(GUILayout.Button("Send"))
            {
                testVFXEvent.SendEvent();
            }
        }
    }
#endif

    public class TestVFXEvent : MonoBehaviour
    {
        [SerializeField] private VisualEffect _visualEffect;
        
        [SerializeField] private string _eventName;

        [SerializeField] private float _range = 1f;

        [SerializeField] private float _size = 1f;
        
        [SerializeField] private float _lifetime = 1f;
        
        [SerializeField] [ColorUsage(true, true)] private Color _color = Color.white;

        public void SendEvent()
        {
            Debug.Log($"Send event : {_eventName} {_color}");

            Vector3 position = this.transform.position + Random.insideUnitSphere * _range;
        
            VFXEventAttribute eventAttribute = _visualEffect.CreateVFXEventAttribute();
        
            eventAttribute.SetVector3("color", new Vector3(_color.r, _color.g, _color.b));
            eventAttribute.SetVector3("position", position);
            eventAttribute.SetFloat("size", _size);
            eventAttribute.SetFloat("lifetime", _lifetime);
        
            _visualEffect.SendEvent(_eventName, eventAttribute);
        }
    }    
}


