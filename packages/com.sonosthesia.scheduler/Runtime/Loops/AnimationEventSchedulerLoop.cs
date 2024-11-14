using System.Linq;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    [CreateAssetMenu(fileName = "AnimationEventLoop", menuName = "Sonosthesia/Scheduler/AnimationEventLoop")]
    public class AnimationEventSchedulerLoop : SchedulerLoop
    {
        [SerializeField] private AnimationClip _clip;

        public override float Duration => _clip.length;
        public override float[] Offsets => _clip.events.Select(e => e.time).ToArray();
    }
}