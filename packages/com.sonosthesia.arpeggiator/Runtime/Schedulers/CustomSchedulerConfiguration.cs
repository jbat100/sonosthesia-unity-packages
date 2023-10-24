using System.Linq;
using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    
    [CreateAssetMenu(fileName = "CustomSchedulerConfiguration", menuName = "Sonosthesia/Scheduling/CustomSchedulerConfiguration")]
    public class CustomSchedulerConfiguration : SchedulerConfiguration
    {
        [SerializeField] private float _duration;

        [SerializeField] private float[] _offsets;

        public override float Duration => _duration;
        public override float[] Offsets => _offsets.ToArray();
    }
}