using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public abstract class AbstractScheduler : ScriptableObject
    {
        public ISchedulerSession CreateSession() => CreateSession(1f, 0f);
        
        public abstract ISchedulerSession CreateSession(float speed, float chaos);
    }
}