using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [RequireComponent(typeof(RigidTransformVelocity))]
    public class CollisionTouchDriver : ContactChannelDriver<TouchPayload>
    {
        protected override bool MakePayload(ContactPoint point, out TouchPayload payload)
        {
            Transform t = transform;
            payload = new TouchPayload(point, t.ToRigidTransform(), Velocity.Velocity);
            return true;
        }
    }
}