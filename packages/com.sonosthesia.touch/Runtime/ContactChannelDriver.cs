using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ContactChannelDriver<TPayload> : CollisionChannelDriver<TPayload> where TPayload : struct
    {
        protected override bool MakePayload(Collision collision, out TPayload payload)
        {
            if (collision.contacts.Length > 0)
            {
                Transform t = transform;
                return MakePayload(collision.contacts[0], out payload);
            }
            payload = default;
            return false;
        }

        protected abstract bool MakePayload(ContactPoint point, out TPayload payload);
    }
}