using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Flow
{
    [CreateAssetMenu(fileName = "RegularSchedulerConfiguration", menuName = "Sonosthesia/RegularSchedulerConfiguration")]
    public class RegularSchedulerConfiguration : SchedulerConfiguration
    {
        [SerializeField] private float _leading;
        [SerializeField] private float _trailing;
        [SerializeField] private float _period;
        [SerializeField] private int _count;

        private List<float> _offsets;

        private bool _initialized;
        
        public override float Duration
        {
            get
            {
                if (_count < 1)
                {
                    return 0;
                }
                return _leading + (_count - 1) * _period + _trailing;
            }
        }

        public override float[] Offsets
        {
            get
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _offsets = new List<float>();
                    if (_count > 0)
                    {
                        float time = _leading;
                        _offsets.Add(time);
                        for (int i = 1; i < _count; i++)
                        {
                            time += _period;
                            _offsets.Add(time);
                        }       
                    }
                }
                return _offsets.ToArray();
            }
        }
    }
}