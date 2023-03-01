using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class FloatVectorAdaptor : SimpleFloatAdaptor<Vector3>
    {
        [SerializeField] private Vector3 _reference;
        
        [SerializeField] private Vector3 _direction;

        protected override Vector3 Map(float value)
        {
            return (value * _direction) + _reference;
        }
    }
}