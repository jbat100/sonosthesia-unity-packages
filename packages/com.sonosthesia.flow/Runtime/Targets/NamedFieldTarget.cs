using System.Reflection;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class NamedFieldTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private Component _component;

        [SerializeField] private string _name;

        private FieldInfo _fieldInfo;
        
        protected void Awake()
        {
            _fieldInfo = _component.GetType().GetField(_name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected override void Apply(T value)
        {
            _fieldInfo.SetValue(_component, value);
        }
    }
}