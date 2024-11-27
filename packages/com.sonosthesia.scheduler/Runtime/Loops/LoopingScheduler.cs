using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public abstract class LoopingScheduler : AbstractScheduler
    {
        [SerializeField] private bool _repeat;

        [SerializeField] private SchedulerLoop _loop;

        public sealed override ISchedulerSession CreateSession(float speed, float chaos)
        {
            return CreateSession(speed, chaos, _repeat, _loop);
        }

        protected abstract ISchedulerSession CreateSession(float speed, float chaos, bool repeat, SchedulerLoop loop);

    }
}