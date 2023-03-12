using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class SchedulerConfiguration : ScriptableObject
    {
        public abstract float Duration { get; }

        public abstract float[] Offsets { get; }
    }
}