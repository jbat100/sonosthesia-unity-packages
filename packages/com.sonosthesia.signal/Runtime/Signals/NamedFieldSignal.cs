using System.Reflection;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class NamedFieldSignal<T> : Signal<T> where T: struct
    {
        [SerializeField] private Component _component;

        [SerializeField] private string _name;

        private FieldInfo _fieldInfo;
        
        protected void Awake()
        {
            _fieldInfo = _component.GetType().GetFieldInHierarchy(_name, 
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected void Update()
        {
            Broadcast((T)_fieldInfo.GetValue(_component));
        }
    }
}