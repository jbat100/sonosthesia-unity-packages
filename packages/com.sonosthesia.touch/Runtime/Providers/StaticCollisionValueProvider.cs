using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch.Providers
{
    public class StaticCollisionValueProvider<T> : CollisionValueProvider<T> where T : struct
    {
        [SerializeField] private T _value;
        
        public override T GetValue(Collision collision, TransformDynamics dynamics)
        {
            return _value;
        }
    }
}