using Sonosthesia.Mapping;
using Sonosthesia.Touch;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Link
{
    public abstract class TouchFaderMapper<T> : FaderLinkMapper<TouchPayload, T> where T : struct
    {
        private enum Driver
        {
            None,
            Distance,
            VerticalDistance,
            HorizontalDistance,
            PositionalVelocity,
            PositionalAcceleration,
            PositionalJerk,
            AngularVelocity,
            AngularAcceleration,
            AngularJerk,
        }
        
        [SerializeField] private Driver _driver;
        
        protected override float Drive(TouchPayload payload)
        {
            return _driver switch
            {
                Driver.Distance => Vector3.Distance(payload.Contact, payload.Source.pos),
                Driver.HorizontalDistance => Vector3.Distance(payload.Contact.Horizontal(), payload.Source.pos.Horizontal()),
                Driver.VerticalDistance => Vector3.Distance(payload.Contact.Vertical(), payload.Source.pos.Vertical()),
                Driver.PositionalVelocity => Vector3.Magnitude(payload.Dynamics.Velocity.Position),
                Driver.PositionalAcceleration => Vector3.Magnitude(payload.Dynamics.Acceleration.Position),
                Driver.PositionalJerk => Vector3.Magnitude(payload.Dynamics.Jerk.Position),
                Driver.AngularVelocity => Vector3.Magnitude(payload.Dynamics.Velocity.Rotation),
                Driver.AngularAcceleration => Vector3.Magnitude(payload.Dynamics.Acceleration.Rotation),
                Driver.AngularJerk => Vector3.Magnitude(payload.Dynamics.Jerk.Rotation),
                _ => 0f
            };
        }
    }
}