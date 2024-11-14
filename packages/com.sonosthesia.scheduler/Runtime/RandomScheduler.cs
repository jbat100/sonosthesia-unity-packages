using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Sonosthesia.Scheduler
{
    [CreateAssetMenu(fileName = "RandomScheduler", menuName = "Sonosthesia/Scheduler/RandomScheduler")]
    public class RandomScheduler : AbstractScheduler
    {
        private class RandomSession : AbstractSchedulerSession
        {
            private static Random _random = new (12345);

            private int _count;
            private float _time;
            private float? _nextEventTime;
            
            protected override void Update()
            {
                _nextEventTime ??= _time + 1 + (_random.NextFloat() * 2f - 1f) * Chaos;
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