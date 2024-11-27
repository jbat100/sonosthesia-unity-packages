using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public abstract class SchedulerLoop : ScriptableObject
    {
        public abstract float Duration { get; }

        public abstract float[] Offsets { get; }
    }
}