using System.Linq;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    
    [CreateAssetMenu(fileName = "CustomSchedulerConfiguration", menuName = "Sonosthesia/Scheduler/CustomLoop")]
    public class CustomSchedulerLoop : SchedulerLoop
    {
        [SerializeField] private float _duration;

        [SerializeField] private float[] _offsets;

        public override float Duration => _duration;
        public override float[] Offsets => _offsets.ToArray();
    }
}