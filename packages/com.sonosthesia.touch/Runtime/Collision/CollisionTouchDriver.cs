using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class CollisionTouchDriver : ContactChannelDriver<TouchPayload>
    {
        private readonly TouchPayloadBuilder _builder = new();
        
        protected override bool MakePayload(Collision collision, ContactPoint point, TransformDynamics dynamics, out TouchPayload payload)
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