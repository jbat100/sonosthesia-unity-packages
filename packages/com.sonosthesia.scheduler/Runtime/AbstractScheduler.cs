using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public abstract class AbstractScheduler : ScriptableObject
    {
        public abstract ISchedulerSession CreateSession(float speed, float chaos);
    }
}