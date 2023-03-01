using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class IntervalScheduler : FrozenScheduler
    {
        [SerializeField] private int _count;

        [SerializeField] private float _interval;

        protected override void InternalTimeOffsets(List<float> buffer)
        {
            float offset = _interval;
            for (int i = 0; i < _count; i++)
            {
                buffer.Add(offset);
                offset += _interval;
            }
        }
    }
}