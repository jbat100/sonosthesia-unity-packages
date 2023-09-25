using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class FloatVector3Adaptor : MapAdaptor<float, Vector3>
    {
        [SerializeField] private Vector3 _offset;
        
        [SerializeField] private Vector3 _direction;

        protected override Vector3 Map(float value) => (value * _direction) + _offset;
    }
}