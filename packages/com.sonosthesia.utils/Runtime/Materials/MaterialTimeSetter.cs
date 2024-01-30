using UnityEngine;

namespace Sonosthesia.Utils
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MaterialTimeSetter))]
    public class MaterialTimeSetterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MaterialTimeSetter setter = (MaterialTimeSetter)target;
            if(GUILayout.Button("Set"))
            {
                setter.Set();
            }
        }
    }
#endif
    
    [RequireComponent(typeof(Renderer))]
    public class MaterialTimeSetter : MonoBehaviour
    {
        [SerializeField] private float _offset;

        [SerializeField] private string _name;
        
        private Renderer _renderer;

        protected virtual void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void Set()
        {
            _renderer.material.SetFloat(_name, Time.time);
        }
    }
}