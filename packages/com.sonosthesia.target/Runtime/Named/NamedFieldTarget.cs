using System.Reflection;
using Sonosthesia.Processing;
using Sonosthesia.Utils;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Target
{
    public class NamedFieldTarget<TValue, TProcessor> : Target<TValue, TProcessor> 
        where TValue : struct where TProcessor : IProcessor<TValue>
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

        protected override void Apply(TValue value)
        {
            _fieldInfo?.SetValue(_component, value);
        }
    }

    public class NamedFieldTarget<TValue> : NamedFieldTarget<TValue, PassthroughProcessor<TValue>> where TValue : struct
    {
        
    }
}