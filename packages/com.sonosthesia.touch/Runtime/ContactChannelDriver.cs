using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ContactChannelDriver<TPayload> : CollisionChannelDriver<TPayload> where TPayload : struct
    {
        protected override bool MakePayload(Collision collision, TransformDynamics dynamics, out TPayload payload)
        {
            if (collision.contacts.Length > 0)
            {
                Transform t = transform;
                return MakePayload(collision, collision.contacts[0], dynamics, out payload);
            }
            payload = default;
            return false;
        }

        protected abstract bool MakePayload(Collision collision, ContactPoint point, TransformDynamics dynamics, out TPayload payload);
    }
}