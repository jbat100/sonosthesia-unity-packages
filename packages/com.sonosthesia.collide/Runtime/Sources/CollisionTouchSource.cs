using Sonosthesia.Dynamic;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class CollisionTouchSource : ContactSource<CollisionTouch>
    {
        private readonly TouchPayloadBuilder _builder = new();
        
        protected override bool MakePayload(Collision collision, ContactPoint point, TransformDynamics dynamics, out CollisionTouch payload)
        {
            _builder.Apply(point);
            _builder.Dynamics = dynamics;
            _builder.Target = transform.ToRigidTransform();
            _builder.Source = collision.transform.ToRigidTransform();
            payload = _builder.ToTouchPayload();
            return true;
        }
    }
}