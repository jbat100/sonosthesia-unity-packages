using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch.Providers
{
    public abstract class DynamicsCollisionValueProvider<T> : CollisionValueProvider<T> where T : struct
    {
        [SerializeField] private TransformDynamics.Domain _domain;

        [SerializeField] private TransformDynamics.Order _order;

        [SerializeField] private float _scale = 1f;
        
        public override T GetValue(Collision collision, TransformDynamics dynamics)
        {
            Vector3 value = dynamics.Select(_order, _domain);
            return Map(value, _scale);
        }

        protected abstract T Map(Vector3 value, float scale);
    }
}