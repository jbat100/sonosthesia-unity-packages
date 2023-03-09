using Sonosthesia.Flow;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchFaderMapper<T> : FaderMapper<TouchPayload, T> where T : struct
    {
        private enum Driver
        {
            None,
            Distance,
            VerticalDistance,
            HorizontalDistance
        }
        
        [SerializeField] private Driver _driver;
        
        protected override float Drive(TouchPayload touchPayload)
        {
            return _driver switch
            {
                Driver.Distance => Vector3.Distance(touchPayload.Contact, touchPayload.Position),
                Driver.HorizontalDistance => Vector3.Distance(touchPayload.Contact.Horizontal(), touchPayload.Position.Horizontal()),
                Driver.VerticalDistance => Vector3.Distance(touchPayload.Contact.Vertical(), touchPayload.Position.Vertical()),
                _ => 0f
            };
        }
    }
}