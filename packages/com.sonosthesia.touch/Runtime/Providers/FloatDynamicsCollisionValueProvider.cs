using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch.Providers
{
    public class FloatDynamicsCollisionValueProvider : DynamicsCollisionValueProvider<float>
    {
        [SerializeField] private Vector3Selector _selector;

        [SerializeField] private bool _abs;
        
        protected override float Map(Vector3 value, float scale)
        {
            float selected = value.Select(_selector);
            return _abs ? Mathf.Abs(selected) : selected;
        }
    }
}