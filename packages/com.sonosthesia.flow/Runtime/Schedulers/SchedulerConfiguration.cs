using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class SchedulerConfiguration : MonoBehaviour
    {
        public abstract float Duration { get; }

        public abstract float[] Offsets { get; }
    }
}