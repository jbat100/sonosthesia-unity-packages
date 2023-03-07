using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchFaderMapper<T> : FaderMapper<TouchPayload, T> where T : struct
    {
        private enum Driver
        {
            None,
            Distance
        }
        
        [SerializeField] private Driver _driver;
        
        protected override float Drive(TouchPayload touchPayload)
        {
            return _driver switch
            {
                Driver.Distance => Vector3.Distance(touchPayload.Contact, touchPayload.Position),
                _ => 0f
            };
        }
    }
}