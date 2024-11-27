using Sonosthesia.Flow;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public class SchedulerPeakRandomAdaptor : MapAdaptor<SchedulerEvent, Peak>
    {
        [SerializeField] private FloatRange _magnitude = new (1f, 1f);
        [SerializeField] private FloatRange _strength = new (1f, 1f);
        [SerializeField] private FloatRange _duration = new (1f, 1f);
        
        protected override Peak Map(SchedulerEvent source) 
            => new (_duration.Random(), _magnitude.Random(), _strength.Random());
    }
}