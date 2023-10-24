using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Flow
{
    [CreateAssetMenu(fileName = "PeriodicSchedulerConfiguration", menuName = "Sonosthesia/Scheduling/PeriodicSchedulerConfiguration")]
    public class PeriodicSchedulerConfiguration : SchedulerConfiguration
    {
        [SerializeField] private float _duration;

        [SerializeField] private float _offset;
        
        [SerializeField] private float _period;

        private List<float> _offsets;
        
        public override float Duration => _duration;

        public override float[] Offsets
        {
            get
            {
                if (_offsets == null)
                {
                    _offsets = new List<float>();
                    float time = _offset;
                    while (time < _duration)
                    {
                        _offsets.Add(time);
                        time += _period;
                    }   
                }

                return _offsets.ToArray();
            }
        }
    }
}