using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class StaticCollisionValueGenerator<T> : CollisionValueGenerator<T> where T : struct
    {
        [SerializeField] private T _value;
        
        public override T GetValue(Collision collision, TransformDynamics dynamics)
        {
            return _value;
        }
    }
}