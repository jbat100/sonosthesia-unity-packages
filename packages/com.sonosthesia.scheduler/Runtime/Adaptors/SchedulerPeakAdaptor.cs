using Sonosthesia.Flow;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public class SchedulerPeakAdaptor : MapAdaptor<SchedulerEvent, Peak>
    {
        [SerializeField] private float _magnitude = 1f;
        [SerializeField] private float _strength = 1f;
        [SerializeField] private float _duration = 1f;
        
        protected override Peak Map(SchedulerEvent source) => new (_duration, _magnitude, _strength);
    }
}