using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    [CreateAssetMenu(fileName = "RandomScheduler", menuName = "Sonosthesia/Scheduler/RandomScheduler")]
    public class RandomScheduler : AbstractScheduler
    {
        private class RandomSession : AbstractSchedulerSession
        {
            private int _count;
            private float _time;
            private float? _nextEventTime;
            
            protected override void Update()
            {
                _nextEventTime ??= _time + 1 + MathUtils.RandomFloat(-1f, 1f) * Chaos;
                _time += Time.deltaTime * Speed;
                
                if (_time < _nextEventTime.Value)
                {
                    return;
                }
                
                Push(new SchedulerEvent(_count, _time, 0));
                _count++;
                _nextEventTime = null;
            }

            public RandomSession(float speed, float chaos) : base(speed, chaos)
            {
            }
        }
        
        public override ISchedulerSession CreateSession(float speed, float chaos) => new RandomSession(speed, chaos);
    }
}