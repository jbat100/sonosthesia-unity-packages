using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    [CreateAssetMenu(fileName = "PeriodicScheduler", menuName = "Sonosthesia/Scheduler/PeriodicScheduler")]
    public class PeriodicScheduler : AbstractScheduler
    {
        private class PeriodicSession : AbstractSchedulerSession
        {
            private float _time;
            private float _nextEventFraction;
            private int _count;

            public PeriodicSession(float speed, float chaos) : base(speed, chaos)
            {
                _nextEventFraction = MathUtils.RandomFloat();
                _time = 0;
            }
            
            protected override void Update()
            {
                _time += Time.deltaTime * Speed;
                int timeFloor = (int)math.floor(_time);

                if (_count > timeFloor)
                {
                    return;
                }
                
                float fraction = _time - timeFloor;

                if (fraction >= _nextEventFraction)
                {
                    Push(new SchedulerEvent(_count, _time, 0f));
                    _nextEventFraction = MathUtils.RandomFloat() * math.clamp(Chaos, 0, 1);
                    _count++;
                }
            }
        }
        
        public override ISchedulerSession CreateSession(float speed, float chaos) => new PeriodicSession(speed, chaos);
    }
}