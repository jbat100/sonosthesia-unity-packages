using System.Reflection;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Target
{
    public class NamedFieldTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private Component _component;

        [SerializeField] private string _name;

        private FieldInfo _fieldInfo;
        
        protected override void Awake()
        {
            _fieldInfo = _component.GetType().GetFieldInHierarchy(_name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            base.Awake();
        }

        protected override void Apply(T value)
        {
            _fieldInfo?.SetValue(_component, value);
        }
    }
}