using System.Reflection;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class NamedFieldSignal<T> : Signal<T> where T: struct
    {
        [SerializeField] private Component _component;

        [SerializeField] private string _name;

        private FieldInfo _fieldInfo;
        
        protected void Awake()
        {
            _fieldInfo = _component.GetType().GetField(_name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected void Update()
        {
            Broadcast((T)_fieldInfo.GetValue(_component));
        }
    }
}